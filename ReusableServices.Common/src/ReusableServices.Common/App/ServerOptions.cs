namespace ReusableServices.Common.App
{
  public class ServerOptions
  {
    public string Name { get; set; }
    public string Scheme { get; set; }
    public string Host { get; set; }
    public string Port { get; set; }

    public string Url() => Url(Port);
    public string Url(string port) => Scheme + "://" + Host + ":" + port;
  }
}