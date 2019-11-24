using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using ReusableServices.Common.App;

namespace ReusableServices.Mails
{
  public static class Program
  {
    public static void Main(string[] args)
    {
      var build = CustomWebHostBuilder.Create<Startup>(CustomWebHostBuilder.AppCommonRoutingFileSettings, args).Build();
      var logger = (ILogger<IWebHost>) build.Services.GetService(typeof(ILogger<IWebHost>));
      try
      {
        logger.LogInformation($"{nameof(Main)}: build running...");
        build.Run();
      }
      catch (Exception ex)
      {
        logger.LogError("Fatal Error", ex);
        throw;
      }
      finally
      {
        logger.LogInformation($"{nameof(Main)}: build stopping...");
        build.StopAsync();
      }
    }
  }
}