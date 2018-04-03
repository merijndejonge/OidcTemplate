using System;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenSoftware.OidcTemplate.Data;

namespace OpenSoftware.OidcTemplate.Auth.DatabaseSeed
{
    public static class WebHostExtensions
    {
        public static void SeedDatabase(this IWebHost host)
        {
            var logger = host.Services.GetRequiredService<ILogger<Program>>();

            using (var scope = host.Services.CreateScope())
            {
                // Migrate and seed the database during startup. Must be synchronous
                try
                {
                    {
                        scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();
                        scope.ServiceProvider.GetService<IdentityContext>().Database.Migrate();
                        scope.ServiceProvider.GetService<ISeedAuthService>().SeedAuthDatabase(scope.ServiceProvider)
                            .Wait();
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Failed to migrate or seed database");
                }
            }
        }
    }
}