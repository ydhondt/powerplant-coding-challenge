using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;

namespace PPCC.Api
{
    /// <summary>
    /// Extension for global error handling.
    /// </summary>
    /// <remarks>
    /// If each action would have to take care of its own exception handling it sooner or later would
    /// be forgotten on one. By using a global approach we guarantee that nothing is missed and we can
    /// provide a standardized reply.
    /// </remarks>
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILogger logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    // We use a guid to identify the issue to the user. The guid prevents information
                    // leaking while still providing an easy lookup mechanism for support.
                    var errorId = Guid.NewGuid().ToString();

                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response.ContentType = "application/json";

                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature != null)
                    {
                        // Log the error with its 'unique' id.
                        logger.LogError($"Error {errorId}: {contextFeature.Error}");

                        // Generate the error response including the 'unique' id.
                        var exceptionInfo = new
                        {
                            StatusCode = context.Response.StatusCode,
                            Message = $"Internal Server Error. Contact support with the following id '{errorId}'."
                        };

                        // Return it.
                        await context.Response.WriteAsync(JsonSerializer.Serialize(exceptionInfo));
                    }
                });
            });
        }
    }
}
