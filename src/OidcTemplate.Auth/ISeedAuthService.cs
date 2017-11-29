using System;
using System.Threading.Tasks;

namespace OpenSoftware.OidcTemplate.Auth
{
    public interface ISeedAuthService
    {
        Task SeedAuthDatabase(IServiceProvider serviceProvider);
    }
}