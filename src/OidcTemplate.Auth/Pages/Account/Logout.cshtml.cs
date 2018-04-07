using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OpenSoftware.OidcTemplate.Auth.Configuration;

namespace OpenSoftware.OidcTemplate.Auth.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly IIdentityServerInteractionService _interactionService;

        public LogoutModel(IIdentityServerInteractionService interactionService)
        {
            _interactionService = interactionService;
        }
        public async Task<IActionResult> OnGetAsync(string logoutId)
        {
            LogoutId = logoutId;

            // build a model so the logout page knows what to display
            var vm = await BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // if the request for logout was properly authenticated from IdentityServer, then
                // we don't need to show the prompt and can just log the user out directly.
                return await Logout(vm);
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(LogoutInputModel model)
        {
            return await Logout(model);
        }
        public string LogoutId { get; set; }

        private async Task<IActionResult> Logout(LogoutInputModel inputModel)
        {
            var model = await GetExternalLogoutModel(inputModel.LogoutId);
            if (model != null)
            {
                // build a return URL so the upstream provider will redirect back
                // to us after the user has logged out. this allows us to then
                // complete our single sign-out processing.
                var url = Url.Page("/Account/Logout", new { logoutId = model.LogoutId });

                // this triggers a redirect to the external provider for sign-out
                return SignOut(new AuthenticationProperties { RedirectUri = url }, model.Scheme);
            }

            return RedirectToPage("/Account/SignedOut", new {inputModel.LogoutId});
        }

        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interactionService.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }
        private async Task<ExternalLogoutModel> GetExternalLogoutModel(string logoutId)
        {
            if (User?.Identity.IsAuthenticated != true) return null;
            var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
            if (idp == null || idp == IdentityServer4.IdentityServerConstants.LocalIdentityProvider) return null;

            var providerSupportsSignout = await HttpContext.GetSchemeSupportsSignOutAsync(idp);
            if (!providerSupportsSignout) return null;
            var model = new ExternalLogoutModel {LogoutId = logoutId};
            if (model.LogoutId == null)
            {
                // if there's no current logout context, we need to create one
                // this captures necessary info from the current logged in user
                // before we signout and redirect away to the external IdP for signout
                model.LogoutId = await _interactionService.CreateLogoutContextAsync();
            }

            model.Scheme = idp;
            return model;
        }
    }

    public class ExternalLogoutModel
    {
        public string LogoutId { get; set; }
        public string Scheme { get; set; }
    }

    public class LogoutInputModel
    {
        public string LogoutId { get; set; }
    }

    public class LogoutViewModel : LogoutInputModel
    {
        public bool ShowLogoutPrompt { get; set; }
    }
}