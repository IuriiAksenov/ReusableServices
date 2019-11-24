using System;
using System.IO;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Hosting;
using ReusableServices.Common.App;
using ReusableServices.Common.Extensions;
using Serilog;
using Serilog.Events;

namespace ReusableServices.Common.Logging
{
  public static class Extensions
  {
    public static IWebHostBuilder UseLogging(this IWebHostBuilder webHostBuilder, string applicationName = null)
    {
      return webHostBuilder.UseSerilog((context, loggerConfiguration) =>
      {
        var appOptions = context.Configuration.GetOptions<ServerOptions>("Server");
        var serilogOptions = context.Configuration.GetOptions<SerilogOptions>("Serilog");
        if (!Enum.TryParse<LogEventLevel>(serilogOptions.MinimalLevel, true, out var level))
          level = LogEventLevel.Information;

        applicationName = string.IsNullOrWhiteSpace(applicationName) ? appOptions.Name : applicationName;
        loggerConfiguration
          .Enrich.FromLogContext().MinimumLevel.Is(level)
          .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
          .Enrich.WithProperty("ApplicationName", applicationName);
        Configure(loggerConfiguration, context, level, serilogOptions.Path, applicationName);
      }, true);
    }

    private static void Configure(LoggerConfiguration loggerConfiguration, WebHostBuilderContext context,
      LogEventLevel level, string logPath, string appName)
    {
      var path = Path.Combine(Path.Combine(logPath, appName), appName + "-{Date}" + ".log");
      const string outputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {SourceContext}.{SourceMemberName}: {Message:lj}{NewLine}{Exception}";
      loggerConfiguration.ReadFrom.Configuration(context.Configuration).MinimumLevel.Is(level).MinimumLevel
        .Override("Microsoft", LogEventLevel.Information)
        .WriteTo.RollingFile(path, outputTemplate: outputTemplate)
        .WriteTo.ColoredConsole(outputTemplate: outputTemplate); //,fileSizeLimitBytes:500*1024*1024);
    }
  }

  public static class LoggerExtensions
  {
    public static ILogger Here(this ILogger logger,
      [CallerMemberName]string callerMemberName = null,
      [CallerFilePath]string callerFilePath = null,
      [CallerLineNumber]int callerLineNumber = 0)
    {
      return logger
        // substituting 'Caller' with 'Source' because 'ForContext<T>()' adds property 'SourceContext'
        .ForContext("SourceMemberName", callerMemberName)
        .ForContext("SourceFilePath", callerFilePath)
        .ForContext("SourceLineNumber", callerLineNumber);
    }
  }

}