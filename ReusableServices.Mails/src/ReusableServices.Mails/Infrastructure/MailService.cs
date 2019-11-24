using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NETCore.MailKit.Core;
using ReusableServices.Common.Vk;
using ReusableServices.Mails.Handlers;
using VkNet;
using VkNet.Model.RequestParams;

namespace ReusableServices.Mails.Infrastructure
{
  public class MailService : IMailService
  {
    private readonly IEmailService _emailService;
    private readonly ILogger<MailService> _logger;

    private readonly ulong _ocsGroupId;
    private readonly Random _rand = new Random();
    private readonly VkApi _vkApi;
    private readonly VkMessageId _vkMessageId;

    public MailService(ILogger<MailService> logger, IEmailService emailService, VkApi vkApi, VkApiOptions vkApiOptions,
      VkMessageId vkMessageId)
    {
      logger.LogInformation($"{nameof(MailService)}.ctr: Begin.");

      _logger = logger;
      _emailService = emailService;

      _ocsGroupId = vkApiOptions.GroupId;
      _vkApi = vkApi;
      _vkMessageId = vkMessageId;
      logger.LogInformation($"{nameof(MailService)}.ctr: End.");
    }

    public async Task SendMailAsync(SendMail mail)
    {
      _logger.LogInformation($"{nameof(MailService)}.{nameof(SendMailAsync)}: Begin.");

      foreach (var email in mail.Emails)
        await _emailService.SendAsync(email, mail.Subject, mail.Body);

      foreach (var vkUserId in mail.VkIds)
        await _vkApi.Messages.SendAsync(new MessagesSendParams
        {
          UserId = vkUserId, GroupId = _ocsGroupId, Message = mail.Body, RandomId = _vkMessageId.GetNewId()
        });

      _logger.LogInformation($"{nameof(MailService)}.{nameof(SendMailAsync)}: End.");
    }
  }
}