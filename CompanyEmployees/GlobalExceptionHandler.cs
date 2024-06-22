using Contracts;
using Entities.ErrorModel;
using Entities.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;

namespace CompanyEmployees
{
  public class GlobalExceptionHandler : IExceptionHandler
  {
    private readonly ILoggerManager _logger;

    public GlobalExceptionHandler(ILoggerManager logger)
    {
      _logger = logger;
    }
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception
      exception, CancellationToken cancellationToken)
    {
      // Sets the status code of the response to 500 (Internal Server Error)
      httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
      // Sets the content type of the response to JSON
      httpContext.Response.ContentType = "application/json";

      // Gets the IExceptionHandlerFeature from the HTTP context
      var contextFeature = httpContext.Features.Get<IExceptionHandlerFeature>();
      if (contextFeature != null)
      {
        // Uses a switch expression to set the status code based on the type of the error
        httpContext.Response.StatusCode = contextFeature.Error switch
        {
          // If the error is a NotFoundException, set the status code to 404 (Not Found)
          NotFoundException => StatusCodes.Status404NotFound,
          // If the error is a bad request exception, set the status code to 400 (bad request)
          BadRequestException => StatusCodes.Status400BadRequest,
          // For all other errors, set the status code to 500 (Internal Server Error)
          _ => StatusCodes.Status500InternalServerError
        };
        // Logs the error message using the logger
        _logger.LogError($"Something went wrong: {exception.Message}");

        // Writes an error response to the HTTP response
        await httpContext.Response.WriteAsync(new ErrorDetails()
        {
          StatusCode = httpContext.Response.StatusCode,
          Message = contextFeature.Error.Message,
        }.ToString());
      }

      // Returns true to indicate that the exception has been handled
      return true;
    }
  }
}
