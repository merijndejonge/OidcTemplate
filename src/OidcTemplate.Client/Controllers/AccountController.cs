using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using OpenSoftware.OidcTemplate.Domain.Configuration;

namespace OpenSoftware.OidcTemplate.Client.Controllers
{
    [Authorize]
    public class AccountController:Controller
    {
        private readonly DomainSettings _domainSettings;

        public AccountController(IOptions<DomainSettings> domainSettings)
        {
            _domainSettings = domainSettings.Value;
        }

        public async Task<IActionResult> RefreshTokens()
        {
            var tokenClient = new TokenClient($"{_domainSettings.Auth.Url}/connect/token",
                _domainSettings.Client.Id,
                _domainSettings.Client.Secret);

            var rt = await HttpContext.GetTokenAsync("refresh_token");
            var tokenResult = await tokenClient.RequestRefreshTokenAsync(rt);
            
            if (tokenResult.IsError)
            {
                return Json(tokenResult.Error);
            }
            var oldIdToken = await HttpContext.GetTokenAsync("id_token");
            var newAccessToken = tokenResult.AccessToken;
            var newRefereshToken = tokenResult.RefreshToken;
            var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(tokenResult.ExpiresIn);


            var tokens = new List<AuthenticationToken>
            {
                new AuthenticationToken{Name = OpenIdConnectParameterNames.IdToken, Value = oldIdToken},
                new AuthenticationToken{Name = OpenIdConnectParameterNames.AccessToken, Value = newAccessToken},
                new AuthenticationToken{Name = OpenIdConnectParameterNames.RefreshToken, Value = newRefereshToken},
                new AuthenticationToken{Name = "expires_at", Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)}
            };

            var info = await HttpContext.AuthenticateAsync("Cookies");
            info.Properties.StoreTokens(tokens);
            await HttpContext.SignInAsync("Cookies", info.Principal, info.Properties);

            return Json(new
            {
                access_token = newAccessToken
            });
        }
        
    }
}