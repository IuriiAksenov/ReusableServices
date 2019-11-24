using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;
using ReusableServices.Common.Extensions;

namespace ReusableServices.Common.Email
{
  public static class Extensions
  {
    private const string SectionName = "Email";

    public static IServiceCollection AddEmail(this IServiceCollection services)
    {
      if (services is null)
        throw new ArgumentNullException($"{nameof(AddEmail)}: {nameof(services)} is null.");

      using var serviceProvider = services.BuildServiceProvider();
      var configuration = serviceProvider.GetService<IConfiguration>();

      services.Configure<EmailOptions>(configuration.GetSection(SectionName));
      var options = configuration.GetOptions<EmailOptions>(SectionName);

      return services.AddMailKit(optionBuilder =>
      {
        optionBuilder.UseMailKit(new MailKitOptions
        {
          Server = options.Server,
          Port = options.Port,
          SenderName = options.SenderName,
          SenderEmail = options.SenderEmail,

          //// can be optional with no authentication
          Account = options.Account,
          Password = options.Password,

          // enable ssl or tls
          Security = true
        });
      });
    }
  }
}