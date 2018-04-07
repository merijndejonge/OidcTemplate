using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Options;
using OpenSoftware.OidcTemplate.Data;
using OpenSoftware.OidcTemplate.Domain.Authentication;
using OpenSoftware.OidcTemplate.Domain.Configuration;

namespace OpenSoftware.OidcTemplate.Auth.Configuration
{
    public class ClientStore : IClientStore
    {
        private readonly IdentityContext _context;
        private readonly DomainSettings _domainSettings;

        public ClientStore(IdentityContext context, IOptions<DomainSettings> domainSettings)
        {
            _context = context;
            _domainSettings = domainSettings.Value;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            switch (clientId)
            {
                case "oidc_web":
                {
                    return Task.FromResult(OidcMvcClient());
                }
            }
            // optionally lookup clientId in database

            return null;
        }

        private Client OidcMvcClient()
        {
            return new Client
            {
                Enabled = true,
                ClientId = _domainSettings.Client.Id,
                ClientName = "OidcTemplate Mvc Client",
                ClientUri = $"{_domainSettings.Client.Url}",
                RequireConsent = false,
                AllowAccessTokensViaBrowser = true,
                AllowOfflineAccess = true,
                AlwaysIncludeUserClaimsInIdToken = false,
                RefreshTokenUsage = TokenUsage.ReUse,
                RefreshTokenExpiration = TokenExpiration.Absolute,
                AbsoluteRefreshTokenLifetime = 157700000, // 5 years. TODO: Consider better approach
                AccessTokenLifetime = 3600,
                AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                FrontChannelLogoutUri = $"{_domainSettings.Client.Url}/signout-oidc",

                ClientSecrets = new List<Secret>
                {
                    new Secret(_domainSettings.Client.Secret.Sha256())
                },
                RedirectUris = new List<string>
                {
                    $"{_domainSettings.Client.Url}/signin-oidc"
                },
                PostLogoutRedirectUris = new List<string>
                {
                    $"{_domainSettings.Client.Url}/signout-callback-oidc"
                },
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    _domainSettings.Api.Id,
                    DomainScopes.Roles,
                    DomainScopes.MvcClientUser,
                    DomainScopes.ApiKeys
                },
                // Embedded in token
                Claims = new List<Claim>
                {
                    new Claim(DomainClaimTypes.LiveEnabled, "true")
                }
            };
        }
    }
}