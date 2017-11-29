using System;
using System.Threading.Tasks;

namespace OpenSoftware.OidcTemplate.Auth.DatabaseSeed
{
    public interface ISeedAuthService
    {
        Task SeedAuthDatabase(IServiceProvider serviceProvider);
    }
}