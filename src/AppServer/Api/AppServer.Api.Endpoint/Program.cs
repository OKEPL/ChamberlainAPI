namespace Chamberlain.AppServer.Api.Endpoint
{
    #region

    using System.IO;

    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;

    using Serilog;

    #endregion

    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The bulid web host
        /// </summary>
        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args).UseKestrel().UseIISIntegration().UseStartup<Startup>()
                .UseContentRoot(Directory.GetCurrentDirectory()).CaptureStartupErrors(true)
                .UseSetting("detailedErrors", "true").UseSerilog(
                    (hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom
                        .Configuration(hostingContext.Configuration).Enrich.FromLogContext()).Build();
        }

       private static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }
    }
}