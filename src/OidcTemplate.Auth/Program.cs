using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using OpenSoftware.OidcTemplate.Auth.DatabaseSeed;
using Serilog;
using Serilog.Events;

namespace OpenSoftware.OidcTemplate.Auth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "OidcTemplate.Auth";

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
            var webHost = CreateWebHostBuilder(args).Build();
            webHost.SeedDatabase();
            webHost.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog()
        ;
    }
}
