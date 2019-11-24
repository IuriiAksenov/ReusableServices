using System.Threading.Tasks;
using ReusableServices.Common.Handlers;
using ReusableServices.Common.RabbitMq;
using ReusableServices.Mails.Infrastructure;

namespace ReusableServices.Mails.Handlers
{
  public class QuestionHandler : ICommandHandler<SendMail>
  {
    private readonly IMailService _mailService;

    public QuestionHandler(IMailService mailService)
    {
      _mailService = mailService;
    }

    public async Task HandleAsync(SendMail command, ICorrelationContext context)
    {
      await _mailService.SendMailAsync(command);
    }
  }
}