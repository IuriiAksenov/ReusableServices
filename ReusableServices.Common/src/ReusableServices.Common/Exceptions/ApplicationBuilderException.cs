using System;
using Microsoft.AspNetCore.Builder;

namespace ReusableServices.Common.Exceptions
{
  public static class ApplicationBuilderExtensions
  {
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
      if (app is null) throw new ArgumentNullException(nameof(app));
      return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
  }
}