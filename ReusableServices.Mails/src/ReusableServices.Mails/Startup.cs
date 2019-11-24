using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReusableServices.Common.Email;
using ReusableServices.Common.Handlers;
using ReusableServices.Common.Mvc;
using ReusableServices.Common.RabbitMq;
using ReusableServices.Common.Vk;
using ReusableServices.Mails.Handlers;
using ReusableServices.Mails.Infrastructure;

namespace ReusableServices.Mails
{
  public class Startup
  {
    private readonly IConfiguration _configuration;
    private readonly ILogger<Startup> _logger;

    public Startup(ILogger<Startup> logger, IConfiguration configuration)
    {
      _logger = logger;
      _configuration = configuration;
    }

    public static void Configure(IApplicationBuilder app)
    {
      app.UseCustomMvc();
      app.UseMvc();
      app.UseRabbitMq().SubscribeCommand<SendMail>();
    }

    public IServiceProvider ConfigureServices(IServiceCollection services)
    {
      _logger.LogInformation($"{nameof(Startup)}.{nameof(ConfigureServices)}: Begin.");
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

      services.AddCustomMvc();
      services.AddEmail();

      services.AddTransient<IMailService, MailService>();

      services.AddOptions();

      services.AddRabbitMq();
      services.AddTransient<ICommandHandler<SendMail>, QuestionHandler>();

      services.AddVkApi();

      _logger.LogInformation($"{nameof(Startup)}.{nameof(ConfigureServices)}: End.");
      return services.BuildServiceProvider();
    }
  }
}