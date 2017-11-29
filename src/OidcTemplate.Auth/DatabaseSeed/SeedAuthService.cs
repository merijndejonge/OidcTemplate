using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenSoftware.OidcTemplate.Domain.Authentication;

namespace OpenSoftware.OidcTemplate.Auth.DatabaseSeed
{
    public class SeedAuthService : ISeedAuthService
    {
        private readonly ILogger<SeedAuthService> _logger;

        public SeedAuthService(ILogger<SeedAuthService> logger)
        {
            _logger = logger;
        }

        public async Task SeedAuthDatabase(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            try
            {
                if (roleManager.Roles.Any() == false)
                {
                    await roleManager.CreateAsync(new IdentityRole(DomainRoles.Admin));

                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to seed authentication database.");
            }
        }
    }
}