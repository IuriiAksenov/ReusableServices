using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using RawRabbit;
using RawRabbit.Common;
using RawRabbit.Enrichers.MessageContext;
using ReusableServices.Common.Exceptions;
using ReusableServices.Common.Extensions;
using ReusableServices.Common.Handlers;
using ReusableServices.Common.Messages;

namespace ReusableServices.Common.RabbitMq
{
  public class BusSubscriber : IBusSubscriber
  {
    private readonly IBusClient _busClient;
    private readonly string _defaultNamespace;
    private readonly ILogger<BusSubscriber> _logger;
    private readonly int _retries;
    private readonly int _retryInterval;
    private readonly IServiceProvider _serviceProvider;

    public BusSubscriber(IApplicationBuilder app)
    {
      _logger = (ILogger<BusSubscriber>) app.ApplicationServices.GetService(typeof(ILogger<BusSubscriber>));
      _serviceProvider = (IServiceProvider) app.ApplicationServices.GetService(typeof(IServiceProvider));
      _busClient = (IBusClient) _serviceProvider.GetService(typeof(IBusClient));
      var configuration = (IConfiguration) _serviceProvider.GetService(typeof(IConfiguration));
      var options = configuration.GetOptions<RabbitMqOptions>("RabbitMq");
      _defaultNamespace = options.Namespace;
      _retries = options.Retries >= 0 ? options.Retries : 3;
      _retryInterval = options.RetryInterval > 0 ? options.RetryInterval : 2;
    }

    public IBusSubscriber SubscribeCommand<TCommand>(string @namespace = null, string queueName = null,
      Func<TCommand, OCSException, IRejectedEvent> onError = null) where TCommand : ICommand
    {
      _busClient.SubscribeAsync<TCommand, CorrelationContext>(async (command, correlationContext) =>
        {
          var commandHandler =
            (ICommandHandler<TCommand>) _serviceProvider.GetService(typeof(ICommandHandler<TCommand>));
          return await TryHandleAsync(command, correlationContext,
            () => commandHandler.HandleAsync(command, correlationContext), onError);
        },
        ctx => ctx.UseSubscribeConfiguration(cfg =>
          cfg.FromDeclaredQueue(q => q.WithName(GetQueueName<TCommand>(@namespace, queueName)))));
      return this;
    }

    public IBusSubscriber SubscribeEvent<TEvent>(string @namespace = null, string queueName = null,
      Func<TEvent, OCSException, IRejectedEvent> onError = null) where TEvent : IEvent
    {
      _busClient.SubscribeAsync<TEvent, CorrelationContext>(async (@event, correlationContext) =>
        {
          var eventHandler = (IEventHandler<TEvent>) _serviceProvider.GetService(typeof(IEventHandler<TEvent>));
          return await TryHandleAsync(@event, correlationContext,
            () => eventHandler.HandleAsync(@event, correlationContext), onError);
        },
        ctx => ctx.UseSubscribeConfiguration(cfg =>
          cfg.FromDeclaredQueue(q => q.WithName(GetQueueName<TEvent>(@namespace, queueName)))));

      return this;
    }

    private async Task<Acknowledgement> TryHandleAsync<TMessage>(TMessage message,
      CorrelationContext correlationContext, Func<Task> handle,
      Func<TMessage, OCSException, IRejectedEvent> onError = null)
    {
      var currentRetry = 0;
      var retryPolicy = Policy.Handle<Exception>()
        .WaitAndRetryAsync(_retries, _ => TimeSpan.FromSeconds(_retryInterval));

      var messageName = message.GetType().Name;

      return await retryPolicy.ExecuteAsync<Acknowledgement>(async () =>
      {
        try
        {
          var retryMessage = currentRetry == 0 ? string.Empty : $"Retry: {currentRetry}.";

          var preLogMessage = $"Handling a message: '{messageName}'" +
                              $"with correlation id: '{correlationContext.Id}'. {retryMessage}";
          _logger.LogInformation(preLogMessage);

          await handle();

          var postLogMessage = $"Handled a message: '{messageName}'" +
                               $"with correlation id: '{correlationContext.Id}'. {retryMessage}";
          _logger.LogInformation(postLogMessage);
          return new Ack();
        }
        catch (Exception exception)
        {
          currentRetry++;
          _logger.LogError(exception, exception.Message);
          if (exception is OCSException ocsException && onError != null)
          {
            var rejectedEvent = onError(message, ocsException);
            await _busClient.PublishAsync(rejectedEvent, ctx => ctx.UseMessageContext(correlationContext));
            _logger.LogInformation($"Published a rejected event: '{rejectedEvent.GetType().Name}' " +
                                   $"for the message '{messageName}' with correlation id: '{correlationContext.Id}'.");
            return new Ack();
          }

          throw new Exception($"Unable to handle a message: '{messageName}' " +
                              $"with correlation id: '{correlationContext.Id}', " +
                              $"retry {currentRetry - 1}/{_retries}...");
        }
      });
    }

    private string GetQueueName<T>(string @namespace = null, string name = null)
    {
      @namespace = string.IsNullOrWhiteSpace(@namespace)
        ? string.IsNullOrWhiteSpace(_defaultNamespace) ? string.Empty : _defaultNamespace
        : @namespace;

      var separatedNamespace = string.IsNullOrWhiteSpace(@namespace) ? string.Empty : $"{@namespace}.";

      return (string.IsNullOrWhiteSpace(name)
        ? $"{Assembly.GetEntryAssembly().GetName().Name}/{separatedNamespace}{typeof(T).Name.Underscore()}"
        : $"{name}/{separatedNamespace}{typeof(T).Name.Underscore()}").ToLowerInvariant();
    }
  }
}