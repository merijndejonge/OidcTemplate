using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OpenSoftware.OidcTemplate.Domain.Configuration;

namespace OpenSoftware.OidcTemplate.Client.ViewComponents
{
    public class AppSettingsViewComponent : ViewComponent
    {
        private readonly DomainSettings _domainSettings;

        public AppSettingsViewComponent(IOptions<DomainSettings> appSettings)
        {
            _domainSettings = appSettings.Value;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var settings = new AppSettingsViewComponentModel
            {
                ApiUrl = _domainSettings.Api.Url,
                AuthUrl = _domainSettings.Auth.Url,
                WebUrl = _domainSettings.Client.Url,
                AccessToken = token,
                UserFullName = HttpContext.User.FindFirstValue(JwtClaimTypes.Name),
                UserEmail = HttpContext.User.FindFirstValue(JwtClaimTypes.Email)
                // ...
            };

            return View(settings);
        }
    }

    public class AppSettingsViewComponentModel
    {
        public string ApiUrl { get; set; }
        public string AuthUrl { get; set; }
        public string AccessToken { get; set; }
        public string WebUrl { get; set; }
        public string UserFullName { get; set; }
        public string UserEmail { get; set; }
    }
}