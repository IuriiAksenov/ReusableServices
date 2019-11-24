namespace ReusableServices.Common.Vk
{
  public class VkApiOptions
  {
    public string AccessToken { get; set; }
    public ulong GroupId { get; set; }
    public string VkMessageIdFile { get; set; }
    public int VkMessageIdQueueCapacity { get; set; }
  }
}