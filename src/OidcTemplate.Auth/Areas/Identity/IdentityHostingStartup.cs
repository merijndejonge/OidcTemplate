using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(OpenSoftware.OidcTemplate.Auth.Areas.Identity.IdentityHostingStartup))]
namespace OpenSoftware.OidcTemplate.Auth.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}