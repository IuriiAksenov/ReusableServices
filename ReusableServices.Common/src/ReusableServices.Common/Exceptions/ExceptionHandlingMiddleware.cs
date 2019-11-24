using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ReusableServices.Common.Response;

namespace ReusableServices.Common.Exceptions
{
  public class ExceptionHandlingMiddleware
  {
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
      _next = next;
    }

    public async Task Invoke(HttpContext context, IHostingEnvironment env, ILogger<ExceptionHandlingMiddleware> logger)
    {
      try
      {
        await _next(context);
      }
      catch (Exception ex)
      {
        logger.LogError(ex, ex.Message, ex.InnerException?.Message);
        await HandleExceptionAsync(context, env, ex);
        if (env.IsDevelopment()) Console.WriteLine(ex);
      }
    }

    private static async Task HandleExceptionAsync(HttpContext context, IHostingEnvironment env, Exception exception)
    {
      var errorCode = "internal_server_error";
      const HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
      var message = exception.Message + exception.InnerException?.Message;
      var stackTrace = env.IsDevelopment() ? exception.StackTrace : string.Empty;

      switch (exception)
      {
        case OCSException ex:
          errorCode = ex.Code;
          break;
      }

      var response = new ResponseObject((HttpStatusCode)context.Response.StatusCode, errorCode, message, new { stackTrace });
      var result = JsonConvert.SerializeObject(response);
      context.Response.ContentType = "application/json";
      context.Response.StatusCode = (int)statusCode;
      await context.Response.WriteAsync(result);
    }
  }
}