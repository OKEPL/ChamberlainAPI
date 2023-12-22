namespace Chamberlain.AppServer.Api.Endpoint.Infrastructure
{
    #region

    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;

    #endregion

    /// <summary>
    /// The simple cors middleware
    /// </summary>
    public class SimpleCorsMiddleware
    {
        private readonly RequestDelegate _next;

        private IHostingEnvironment _environment;

        /// <summary>
        /// The declare knext to request delegate and environment to interface hosting environment
        /// </summary>
        public SimpleCorsMiddleware(RequestDelegate next, IHostingEnvironment environment)
        {
            this._next = next;
            this._environment = environment;
        }

        /// <summary>
        /// The invoke context to http context
        /// </summary>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                context.Response.Headers.Add(
                    "Access-Control-Allow-Origin",
                    new[] { (string)context.Request.Headers["Origin"] });
                context.Response.Headers.Add(
                    "Access-Control-Allow-Headers",
                    new[] { "Origin, X-Requested-With, Content-Type, Accept, Authorization" });
                context.Response.Headers.Add(
                    "Access-Control-Allow-Methods",
                    new[] { "GET, POST, PUT, DELETE, OPTIONS" });
                context.Response.Headers.Add("Access-Control-Allow-Credentials", new[] { "true" });

                if (context.Request.Method == "OPTIONS")
                {
                    context.Response.StatusCode = 200;
                    await context.Response.WriteAsync("OK");
                }
                else
                {
                    await this._next(context);
                }
            }
            catch (Exception)
            {
                await this._next(context);
            }
        }
    }

    /// <summary>
    /// The options middleware extensions
    /// </summary>
    public static class OptionsMiddlewareExtensions
    {
        /// <summary>
        /// The use simple cors and return bulider use middleware
        /// </summary>
        public static IApplicationBuilder UseSimpleCors(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SimpleCorsMiddleware>();
        }
    }
}