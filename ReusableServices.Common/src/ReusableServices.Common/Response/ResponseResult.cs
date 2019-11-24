using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ReusableServices.Common.Response
{
  public abstract class ResponseResult : IActionResult
  {
    protected readonly ResponseObject Response;

    protected ResponseResult(HttpStatusCode status, string code, string message) : this(status, code, message, null)
    {
    }

    protected ResponseResult(HttpStatusCode status, string code, string message = null, object data = null)
    {
      Response = new ResponseObject(status, code, message, data);
    }

    public async Task ExecuteResultAsync(ActionContext context)
    {
      var serializerSettings =
        new JsonSerializerSettings {ContractResolver = new CamelCasePropertyNamesContractResolver()};
      context.HttpContext.Response.ContentType = "application/json";
      context.HttpContext.Response.StatusCode = (int) ChangeStatusCodeToSaveBody(Response.Status);
      await context.HttpContext.Response.WriteAsync(JsonConvert.SerializeObject(Response, serializerSettings));
    }

    private static HttpStatusCode ChangeStatusCodeToSaveBody(HttpStatusCode httpStatusCode)
    {
      switch (httpStatusCode)
      {
        case HttpStatusCode.NoContent: return HttpStatusCode.Ok;
        case HttpStatusCode.NotModified: return HttpStatusCode.Ok;
        //case HttpStatusCode.BadRequest: return HttpStatusCode.OK;
        //case HttpStatusCode.NotModified:
        //  return HttpStatusCode.;
        default: return httpStatusCode;
      }
    }
  }

  public class OkResponseResult : ResponseResult
  {
    public OkResponseResult(string message, object data = null) : this(HttpStatusCode.Ok.ToString(), message, data)
    {
    }

    public OkResponseResult(string code, string message, object data = null) : base(HttpStatusCode.Ok, code, message,
      data)
    {
    }
  }

  public class ForbiddenResponseResult : ResponseResult
  {
    public ForbiddenResponseResult(string message, object data = null) : this(HttpStatusCode.Forbidden.ToString(), message, data)
    {
    }

    public ForbiddenResponseResult(string code, string message, object data = null) : base(HttpStatusCode.Forbidden, code, message,
      data)
    {
    }
  }

  public class CreatedResponseResult : ResponseResult
  {
    public CreatedResponseResult(string message, object data = null) : this(HttpStatusCode.Created.ToString(), message, data)
    {
    }

    public CreatedResponseResult(string code, string message, object data = null) : base(HttpStatusCode.Created, code, message,
      data)
    {
    }
  }

  public class UpdatedResponseResult : ResponseResult
  {
    public UpdatedResponseResult(string message, object data = null) : this(HttpStatusCode.Updated.ToString(), message,
      data)
    {
    }

    public UpdatedResponseResult(string code, string message, object data = null) : base(HttpStatusCode.Updated, code,
      message, data)
    {
    }
  }

  public class NoContentResponseResult : ResponseResult
  {
    public NoContentResponseResult(string message, object data = null) : this(HttpStatusCode.NoContent.ToString(),
      message, data)
    {
    }

    public NoContentResponseResult(string code, string message, object data = null) : base(HttpStatusCode.NoContent,
      code, message, data)
    {
    }
  }

  public class NotFoundResponseResult : ResponseResult
  {
    public NotFoundResponseResult(string message, object data = null) : this(HttpStatusCode.NotFound.ToString(),
      message, data)
    {
    }

    public NotFoundResponseResult(string code, string message, object data = null) : base(HttpStatusCode.NotFound, code,
      message, data)
    {
    }
  }

  public class BadResponseResult : ResponseResult
  {
    public BadResponseResult(string message, object data = null) : base(HttpStatusCode.BadRequest,
      HttpStatusCode.BadRequest.ToString(), message, data)
    {
    }

    public BadResponseResult(string code, string message, object data = null) : base(HttpStatusCode.BadRequest, code,
      message, data)
    {
    }

    public BadResponseResult(ModelStateDictionary modelState) : base(HttpStatusCode.BadRequest, HttpStatusCode.BadRequest.ToString())
    {
      if (modelState.IsValid) throw new ArgumentException("ModelState must be invalid", nameof(modelState));

      var errors = modelState
        .Select(x => new {x.Key, Message = x.Value.Errors.Select(error => error.ErrorMessage).ToList()}).ToList();
      Response.Data = new {Errors = errors};
    }
  }
}