using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using ReusableServices.Common.CommandLine;
using ReusableServices.Common.Extensions;
using ReusableServices.Common.Logging;

namespace ReusableServices.Common.App
{
  public static class CustomWebHostBuilder
  {
    public static readonly string[] AppCommonRoutingFileSettings = { AppSettingsFile };
    public static string AppSettingsFile => "appsettings.json";

    public static IWebHostBuilder Create<TStartup>(string fileSetting, string[] args) where TStartup : class
    {
      return Create<TStartup>(new[] { fileSetting }, args);
    }

    public static IWebHostBuilder Create<TStartup>(string fileSetting1, string fileSetting2, string[] args)
      where TStartup : class
    {
      return Create<TStartup>(new[] { fileSetting1, fileSetting2 }, args);
    }

    public static IWebHostBuilder Create<TStartup>(string fileSetting1, string fileSetting2, string fileSetting3,
      string[] args) where TStartup : class
    {
      return Create<TStartup>(new[] { fileSetting1, fileSetting2, fileSetting3 }, args);
    }

    public static IWebHostBuilder Create<TStartup>(string fileSetting1, string fileSetting2, string fileSetting3,
      string fileSetting4, string[] args) where TStartup : class
    {
      return Create<TStartup>(new[] { fileSetting1, fileSetting2, fileSetting3, fileSetting4 }, args);
    }

    public static IWebHostBuilder Create<TStartup>(string fileSetting1, string fileSetting2, string fileSetting3,
      string fileSetting4, string fileSetting5, string[] args) where TStartup : class
    {
      return Create<TStartup>(new[] { fileSetting1, fileSetting2, fileSetting3, fileSetting4, fileSetting5 }, args);
    }

    public static IWebHostBuilder Create<TStartup>(string[] fileSettings, string[] args) where TStartup : class
    {
      var commandLine = Command.WithName("host").HasOption("-port", "-p").HasOption("-swagger", "-swg")
        .HasOption("-currentdirectory", "-curdir");

      var port = string.Empty;
      if (commandLine.TryParse(args, out var command) && command is HostCommand hostCommand)
      {
        if (!string.IsNullOrEmpty(hostCommand.CurrentDirectory))
        {
          var dir = Path.GetDirectoryName(hostCommand.CurrentDirectory);
          Directory.SetCurrentDirectory(dir);
        }

        port = hostCommand.Port;
      }

      var builder = new ConfigurationBuilder().SetBasePath(AppContext.BaseDirectory);

      foreach (var fileSetting in fileSettings) builder.AddJsonFile(fileSetting, false, true);

      var configuration = builder.Build();
      var serverOptions = configuration.GetOptions<ServerOptions>("Server");
      var url = serverOptions.Url();

      if (!string.IsNullOrEmpty(port)) url = serverOptions.Url(port);

      return WebHost.CreateDefaultBuilder(args).UseConfiguration(configuration).UseUrls(url).UseLogging()
        .UseStartup<TStartup>();
    }
  }
}