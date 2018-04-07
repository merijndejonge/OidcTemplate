using System.Threading.Tasks;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenSoftware.OidcTemplate.Auth.Configuration;
using OpenSoftware.OidcTemplate.Domain.Entities;

namespace OpenSoftware.OidcTemplate.Auth.Pages.Account
{
    public class SignedOutModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interactionService;

        public SignedOutModel(SignInManager<ApplicationUser> signInManager, IIdentityServerInteractionService interactionService)
        {
            _signInManager = signInManager;
            _interactionService = interactionService;
        }
        public async Task<IActionResult> OnGetAsync(string logoutId)
        {
            await BuildLoggedOutViewModelAsync(logoutId);

            if (User?.Identity.IsAuthenticated == true)
            {
                // delete local authentication cookie
                await _signInManager.SignOutAsync();
            }

            return Page();
        }

        private async Task BuildLoggedOutViewModelAsync(string logoutId)
        {
            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interactionService.GetLogoutContextAsync(logoutId);

            AutomaticRedirectAfterSignOut = AccountOptions.AutomaticRedirectAfterSignOut;
            PostLogoutRedirectUri = logout?.PostLogoutRedirectUri ?? AccountOptions.DefaultRedirect;
            ClientName = string.IsNullOrEmpty(logout?.ClientName) ? logout?.ClientId : logout.ClientName;
            SignOutIframeUrl = logout?.SignOutIFrameUrl;
        }

        [BindProperty]
        public bool AutomaticRedirectAfterSignOut { get; set; }
        [BindProperty]
        public string PostLogoutRedirectUri { get; set; }
        [BindProperty]
        public string ClientName { get; set; }
        [BindProperty]
        public string SignOutIframeUrl { get; set; }
    }
}