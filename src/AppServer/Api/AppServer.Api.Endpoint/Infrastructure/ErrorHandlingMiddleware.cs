namespace Chamberlain.AppServer.Api.Endpoint.Infrastructure
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Chamberlain.AppServer.Api.Contracts.Exceptions;
    using Chamberlain.Common.Akka;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    /// <summary>
    /// The class error handling middleware
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// The request delegate next
        /// </summary>
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        /// The invoke to http context
        /// </summary>
        public async Task Invoke(HttpContext context /* other scoped dependencies */)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            var message = exception.Message;

            switch (exception)
            {
                case ApiException apiException:
                    message = apiException.Result.Message;
                    var type = Type.GetType(apiException.Result.Type);

                    // ReSharper disable once AssignNullToNotNullAttribute
                    // TODO Create proper exceptions for all codes
                    var innerException = Activator.CreateInstance(type);
                    switch (innerException)
                    {
                        case HttpUnauthorizedException _:
                            code = HttpStatusCode.Unauthorized;
                            break;
                        case ArgumentNullException _:
                        case InvalidOperationException _:
                            code = HttpStatusCode.NotFound;
                            break;
                        case ArgumentException _:
                            code = HttpStatusCode.BadRequest;
                            break;
                        case HttpConflictException _:
                            code = HttpStatusCode.Conflict;
                            break;
                    }

                    break;

                case ArgumentNullException _:
                    message = exception.Message;
                    code = HttpStatusCode.NotFound;
                    break;
                case ArgumentException _:
                    message = exception.Message;
                    code = HttpStatusCode.BadRequest;
                    break;
            }

            var result = JsonConvert.SerializeObject(new { error = message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}