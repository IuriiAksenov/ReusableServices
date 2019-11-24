using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Configuration;
using RawRabbit.DependencyInjection.ServiceCollection;
using RawRabbit.Enrichers.MessageContext;
using RawRabbit.Instantiation;
using ReusableServices.Common.Extensions;
using ReusableServices.Common.Handlers;
using ReusableServices.Common.Messages;

namespace ReusableServices.Common.RabbitMq
{
  public static class Extensions
  {
    private const string SectionName = "RabbitMq";

    public static IBusSubscriber UseRabbitMq(this IApplicationBuilder app)
    {
      if (app is null)
        throw new ArgumentNullException($"{nameof(UseRabbitMq)}: {nameof(app)} is null.");

      return new BusSubscriber(app);
    }

    public static IServiceCollection AddRabbitMq(this IServiceCollection services)
    {
      if (services is null)
        throw new ArgumentNullException($"{nameof(AddRabbitMq)}: {nameof(services)} is null.");

      using var serviceProvider = services.BuildServiceProvider();
      var configuration = serviceProvider.GetService<IConfiguration>();

      services.Configure<RabbitMqOptions>(configuration.GetSection(SectionName));
      services.Configure<RawRabbitConfiguration>(configuration.GetSection(SectionName));
      services.AddSingleton<IHandler, Handler>();
      services.AddSingleton<IBusPublisher, BusPublisher>();

      ConfigureBus(services, configuration);

      return services;
    }

    private static void ConfigureBus(IServiceCollection services, IConfiguration configuration)
    {
      var rawRabbitOptions = configuration.GetOptions<RabbitMqOptions>(SectionName);
      var rawRabbitConfiguration = configuration.GetOptions<RawRabbitConfiguration>(SectionName);
      var namingConventions = new CustomNamingConventions(rawRabbitOptions.Namespace);
      var options = new RawRabbitOptions
      {
        DependencyInjection = ioc =>
        {
          ioc.AddSingleton(rawRabbitOptions);
          ioc.AddSingleton(rawRabbitConfiguration);
          ioc.AddSingleton<INamingConventions>(namingConventions);
        },
        Plugins = p => p.UseAttributeRouting().UseMessageContext<CorrelationContext>().UseContextForwarding()
      };
      services.AddRawRabbit(options);
    }

    private sealed class CustomNamingConventions : NamingConventions
    {
      public CustomNamingConventions(string defaultNamespace)
      {
        ExchangeNamingConvention = type => GetNamespace(type, defaultNamespace).ToLowerInvariant();
        RoutingKeyConvention = type =>
          $"#.{GetRoutingKeyNamespace(type, defaultNamespace)}{type.Name.Underscore()}".ToLowerInvariant();
        ErrorExchangeNamingConvention = () => $"{defaultNamespace}.error";
        RetryLaterExchangeConvention = span => $"{defaultNamespace}.retry";
        RetryLaterQueueNameConvetion = (exchange, span) =>
          $"{defaultNamespace}.retry_for_{exchange.Replace(".", "_")}_in_{span.TotalMilliseconds}_ms"
            .ToLowerInvariant();
      }

      private static string GetRoutingKeyNamespace(Type type, string defaultNamespace)
      {
        var @namespace = type.GetCustomAttribute<MessageNamespaceAttribute>()?.Namespace ?? defaultNamespace;

        return string.IsNullOrWhiteSpace(@namespace) ? string.Empty : $"{@namespace}.";
      }

      private static string GetNamespace(Type type, string defaultNamespace)
      {
        var @namespace = type.GetCustomAttribute<MessageNamespaceAttribute>()?.Namespace ?? defaultNamespace;

        return string.IsNullOrWhiteSpace(@namespace) ? "#" : $"{@namespace}";
      }
    }
  }
}