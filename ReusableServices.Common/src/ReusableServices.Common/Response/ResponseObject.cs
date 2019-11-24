using Newtonsoft.Json;

namespace ReusableServices.Common.Response
{
  public enum HttpStatusCode
  {
    Ok = System.Net.HttpStatusCode.OK,
    Created = System.Net.HttpStatusCode.Created,
    NoContent = System.Net.HttpStatusCode.NoContent,
    Updated = System.Net.HttpStatusCode.Accepted,
    NotModified = System.Net.HttpStatusCode.NotModified,
    BadRequest = System.Net.HttpStatusCode.BadRequest,
    Unauthorized = System.Net.HttpStatusCode.Unauthorized,
    Forbidden = System.Net.HttpStatusCode.Forbidden,
    NotFound = System.Net.HttpStatusCode.NotFound,
    Conflict = System.Net.HttpStatusCode.Conflict,
    InternalServerError = System.Net.HttpStatusCode.InternalServerError
  }

  public class ResponseObject
  {
    [JsonIgnore]
    public HttpStatusCode Status { get; }

    [JsonProperty("code")]
    public string Code { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("data")]
    public object Data { get; set; }

    public ResponseObject(HttpStatusCode status, string code, string message, object data)
    {
      Message = string.IsNullOrEmpty(message) ? GetDefaultMessage(status) : message;
      Status = status;
      Code = code;
      Data = data;
    }

    private static string GetDefaultMessage(HttpStatusCode statusCode)
    {
      switch (statusCode)
      {
        case HttpStatusCode.Ok: return "ok";
        case HttpStatusCode.Created: return "created";
        case HttpStatusCode.NoContent: return "not_content";
        case HttpStatusCode.Updated: return "updated";
        case HttpStatusCode.NotModified: return "not_modified";
        case HttpStatusCode.BadRequest: return "bad_request";
        case HttpStatusCode.Unauthorized: return "unauthorized";
        case HttpStatusCode.Forbidden: return "forbidden";
        case HttpStatusCode.NotFound: return "not_found";
        case HttpStatusCode.Conflict: return "conflict";
        case HttpStatusCode.InternalServerError: return "internal_server_error";
        default: return "default_internal_server_error";
      }
    }
  }

  public class ResponseObject<T> : ResponseObject
  {
    [JsonProperty("data")]
    public new T Data { get; set; }

    public ResponseObject(HttpStatusCode status, string code, string message, T data) : base(status, code, message, data)
    {
      Data = data;
    }
  }
}