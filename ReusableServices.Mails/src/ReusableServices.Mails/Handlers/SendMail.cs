using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using ReusableServices.Common.Messages;

namespace ReusableServices.Mails.Handlers
{
  public class SendMail : ICommand
  {
    public List<string> Emails { get; }
    public List<long> VkIds { get; }

    [Required]
    public string Subject { get; }

    [Required]
    public string Body { get; }

    [JsonConstructor]
    public SendMail(string subject, string body, List<string> emails, List<long> vkIds)
    {
      Subject = subject;
      Body = body;
      Emails = emails ?? new List<string>();
      VkIds = vkIds ?? new List<long>();
    }
  }
}