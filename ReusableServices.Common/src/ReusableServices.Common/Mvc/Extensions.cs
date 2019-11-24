using System;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.WebEncoders;
using ReusableServices.Common.App;
using ReusableServices.Common.Exceptions;
using ReusableServices.Common.Extensions;
using ReusableServices.Common.Swagger;

namespace ReusableServices.Common.Mvc
{
  public class CustomMvcOptions
  {
    public string HubSegment { get; set; }
  }

  public static class Extensions
  {
    public static IServiceCollection AddProduces(this IServiceCollection services)
    {
      if (services is null)
        throw new ArgumentNullException($"{nameof(AddProduces)}: {nameof(services)} is null.");

      services.Configure<MvcOptions>(options => { options.Filters.Add(new ProducesAttribute("application/json")); });
      return services;
    }

    public static IServiceCollection AddAllowAllOrigin(this IServiceCollection services)
    {
      if (services is null)
        throw new ArgumentNullException($"{nameof(AddAllowAllOrigin)}: {nameof(services)} is null.");

      services.AddCors(options =>
      {
        options.AddPolicy("AllowAllOrigin", builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
      });
      services.Configure<MvcOptions>(options =>
      {
        options.Filters.Add(new CorsAuthorizationFilterFactory("AllowAllOrigin"));
      });
      return services;
    }

    public static IServiceCollection AddCustomMvc(this IServiceCollection services)
    {
      if (services is null)
        throw new ArgumentNullException($"{nameof(AddCustomMvc)}: {nameof(services)} is null.");

      return AddCustomMvc(services, new CustomMvcOptions());
    }

    public static IServiceCollection AddCustomMvc(this IServiceCollection services, CustomMvcOptions customMvcOptions)
    {
      if (services is null)
        throw new ArgumentNullException($"{nameof(AddCustomMvc)}: {nameof(services)} is null.");

      using var serviceProvider = services.BuildServiceProvider();
      var configuration = serviceProvider.GetService<IConfiguration>();

      services.AddProduces();
      services.AddAllowAllOrigin();

      services.AddSwaggerDocs();

      services.AddOptions();
      services.Configure<ServerOptions>(configuration.GetSection("Server"));
      services.AddSingleton(configuration.GetOptions<ServerOptions>("Server"));

      services.Configure<WebEncoderOptions>(options =>
        options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

      return services;
    }

    public static IApplicationBuilder UseCustomMvc(this IApplicationBuilder app)
    {
      app.UseForwardedHeaders(new ForwardedHeadersOptions
      {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
      });
      app.UseStaticFiles();
      app.UseDefaultFiles();
      app.UseSwaggerDocs();
      app.UseExceptionHandling();
      app.UseAuthentication();
      return app;
    }
  }
}