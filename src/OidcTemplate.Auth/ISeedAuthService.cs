using System.Threading.Tasks;

namespace OpenSoftware.OidcTemplate.Auth
{
    public interface ISeedAuthService
    {
        Task SeedAuthDatabase();
    }

    public class SeedAuthService : ISeedAuthService
    {
        public Task SeedAuthDatabase()
        {
            return Task.CompletedTask;
        }
    }
}