using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReusableServices.Common.Response;
using ReusableServices.Mails.Handlers;
using ReusableServices.Mails.Infrastructure;

namespace ReusableServices.Mails.Controllers
{
  [Route("mails")]
  [ApiController]
  public class MailsController : ControllerBase
  {
    private readonly IMailService _mailService;

    public MailsController(IMailService mailService)
    {
      _mailService = mailService;
    }

    [HttpPost]
    public async Task<IActionResult> SendMailAsync([FromBody]
      SendMail sendMail)
    {
      if (!ModelState.IsValid)
        return new BadResponseResult(ModelState);

      await _mailService.SendMailAsync(sendMail);
      return new OkResponseResult(Codes.MailSent);
    }
  }
}