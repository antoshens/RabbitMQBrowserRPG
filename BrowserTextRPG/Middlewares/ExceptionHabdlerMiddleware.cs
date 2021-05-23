using BrowserTextRPG.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace BrowserTextRPG.Middlewares
{
    public class ExceptionHabdlerMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHabdlerMiddleware> _logger;

        public ExceptionHabdlerMiddleware(ILogger<ExceptionHabdlerMiddleware> logger)
        {
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await ExceptionHandler(context, ex);
            }
        }

        private async Task ExceptionHandler(HttpContext context, Exception exception)
        {
            var response = new GatewayResponse<object>();

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            response.Fault = new Fault
            {
                ErrorCode = exception.HResult,
                ErrorMessage = exception.Message
            };

            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));

            this._logger.LogError($"Error in {context.GetEndpoint().DisplayName}: {exception.Message}.");
        }
    }
}
