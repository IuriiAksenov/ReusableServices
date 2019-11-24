using System.Threading.Tasks;
using ReusableServices.Mails.Handlers;

namespace ReusableServices.Mails.Infrastructure
{
  public interface IMailService
  {
    Task SendMailAsync(SendMail sendMailAboutMissedCall);
  }
}