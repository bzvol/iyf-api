using System.Net.Mime;
using System.Text.Json;
using IYFApi.Models.Response;
using Microsoft.AspNetCore.Diagnostics;

namespace IYFApi;

public static class ExceptionHandling
{
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
                Error = exception?.Message,
                StackTrace = exception?.StackTrace
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(result));
        });

    private static int GetStatusCodeForException(Exception? exception) =>
        exception switch
        {
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

    private static string GetMessageForException(Exception? exception) =>
        exception switch
        {
            KeyNotFoundException => "The specified resource could not be found.",
            null => "An unknown error has occurred.",
            _ => "An error has occurred."
        };
}