using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ReusableServices.Common.App;
using ReusableServices.Common.Extensions;
using Swashbuckle.AspNetCore.Swagger;

namespace ReusableServices.Common.Swagger
{
  public static class Extensions
  {
    public static IServiceCollection AddSwaggerDocs(this IServiceCollection services)
    {
      if (services is null)
        throw new ArgumentNullException($"{nameof(AddSwaggerDocs)}: {nameof(services)} is null.");

      using var serviceProvider = services.BuildServiceProvider();
      var configuration = serviceProvider.GetService<IConfiguration>();

      services.Configure<SwaggerOptions>(configuration.GetSection("Swagger"));
      var swaggerOptions = configuration.GetOptions<SwaggerOptions>("Swagger");
      var serverOptions = configuration.GetOptions<ServerOptions>("Server");

      if (!swaggerOptions.Enabled)
      {
        return services;
      }

      return services.AddSwaggerGen(c =>
      {
        c.CustomSchemaIds(DefaultSchemaIdSelector);
        c.SwaggerDoc(swaggerOptions.Name, new Info { Title = swaggerOptions.Title + $" for '{serverOptions.Name}'", Version = swaggerOptions.Version, Description = swaggerOptions.Description + $" for '{serverOptions.Name}'" });
        if (swaggerOptions.IncludeSecurity)
        {
          c.AddSecurityDefinition("Bearer", new ApiKeyScheme
          {
            Description =
              "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = "header",
            Type = "apiKey"
          });
          c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>> { { "Bearer", Array.Empty<string>() } });
        }

        // Set the comments path for the Swagger JSON and UI.
        var xmlFile = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
      });
    }

    public static IApplicationBuilder UseSwaggerDocs(this IApplicationBuilder builder)
    {
      var options = builder.ApplicationServices.GetService<IConfiguration>()
        .GetOptions<SwaggerOptions>("Swagger");
      if (!options.Enabled)
      {
        return builder;
      }

      var routePrefix = string.IsNullOrWhiteSpace(options.RoutePrefix) ? "swagger" : options.RoutePrefix;

      builder.UseStaticFiles();
      builder.UseSwagger(c => c.RouteTemplate = routePrefix + "/{documentName}/swagger.json");

      return options.ReDocEnabled
        ? builder.UseReDoc(c =>
        {
          c.RoutePrefix = routePrefix;
          c.SpecUrl = $"{options.Name}/swagger.json";
        })
        : builder.UseSwaggerUI(c =>
        {
          c.SwaggerEndpoint($"/{routePrefix}/{options.Name}/swagger.json", options.Title);
          //c.SwaggerEndpoint($"/swagger/v1/swagger.json", options.Title);
          c.RoutePrefix = routePrefix;
        });
    }

    private static string DefaultSchemaIdSelector(Type modelType)
    {
      if (!modelType.IsConstructedGenericType) return modelType.Name;

      var prefix = modelType.GetGenericArguments()
        .Select(DefaultSchemaIdSelector)
        .Aggregate((previous, current) => previous + current);

      return prefix + modelType.Name.Split('`').First();
    }
  }
}