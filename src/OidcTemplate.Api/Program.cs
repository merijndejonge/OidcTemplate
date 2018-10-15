using System;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Events;

namespace OpenSoftware.OidcTemplate.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "OidcTemplate.Api";

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Verbose)
                .MinimumLevel.Override("IdentityServer4", LogEventLevel.Debug)
                .Enrich.FromLogContext()
                .WriteTo.Console(
                    outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}"
                )
                .CreateLogger();


            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog().UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, 5101);
                    options.Listen(IPAddress.Loopback, 5001, listenOptions => { listenOptions.UseHttps(); });
                });
    }
}
