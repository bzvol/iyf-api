using System.Net.Mime;
using System.Text.Json;
using IYFApi.Models.Response;
using Microsoft.AspNetCore.Diagnostics;

namespace IYFApi;

public static class ExceptionHandling
{
    public static bool IsDevelopment { private get; set; } 
    
    public static void ExceptionHandler(IApplicationBuilder app) =>
        app.Run(async context =>
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;

            var exceptionHandlerPathFeature =
                context.Features.Get<IExceptionHandlerPathFeature>();
            var exception = exceptionHandlerPathFeature?.Error;

            context.Response.StatusCode = GetStatusCodeForException(exception);

            var result = new ErrorResponse
            {
                Message = GetMessageForException(exception),
                Error = exception != null 
                    ? exception.GetType().Name + ": " + exception.Message 
                    : null,
                StackTrace = IsDevelopment ? exception?.StackTrace : null
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(result));
        });

    private static int GetStatusCodeForException(Exception? exception) =>
        exception switch
        {
            InvalidOperationException => StatusCodes.Status400BadRequest,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            NotImplementedException => StatusCodes.Status501NotImplemented,
            _ => StatusCodes.Status500InternalServerError
        };

    private static string GetMessageForException(Exception? exception) =>
        exception switch
        {
            InvalidOperationException => "The request was invalid.",
            KeyNotFoundException => "The specified resource could not be found.",
            NotImplementedException => "The requested functionality is not yet implemented. Contact the developer: https://github.com/bzvol/iyf-api/issues",
            null => "An unknown error has occurred.",
            _ => "An error has occurred."
        };
}