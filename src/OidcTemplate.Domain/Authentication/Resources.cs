using System.Collections.Generic;
using IdentityServer4.Models;
using OpenSoftware.OidcTemplate.Domain.Configuration;

namespace OpenSoftware.OidcTemplate.Domain.Authentication
{
    public static class Resources
    {
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource
                {
                    Name = DomainScopes.MvcClientUser,
                    DisplayName = "MVC client user",
                    Description = "Basic resource, to be adjusted for your application",
                    UserClaims = new List<string>
                    {
                        DomainClaimTypes.TestUserId,
                        DomainClaimTypes.LiveUserId,
                        DomainClaimTypes.LiveEnabled,
                        DomainClaimTypes.SomeClaim,
                        DomainClaimTypes.AnotherClaim
                    }
                },
                new IdentityResource(DomainScopes.Roles, new List<string> {DomainClaimTypes.Role})
            };
        }

        public static IEnumerable<ApiResource> GetApis(WebResource api)
        {
            return new List<ApiResource>
            {
                new ApiResource
                {
                    Name = api.Id,
                    DisplayName = "API 1",
                    Description = "The cool API you want to protect",
                    ApiSecrets = new List<Secret> {new Secret(api.Secret.Sha256())},
                    UserClaims = new List<string>
                    {
                        DomainClaimTypes.Role,
                        DomainClaimTypes.TestUserId,
                        DomainClaimTypes.LiveUserId,
                        DomainClaimTypes.LiveEnabled,
                        DomainClaimTypes.SomeClaim
                    },
                    Scopes = new List<Scope>
                    {
                        new Scope(api.Id), // Should match name of ApiResource.Name
                        new Scope
                        {
                            Name = DomainScopes.ApiKeys,
                            DisplayName = "API Keys",
                            Description = "Access to the API keys."
                        }
                    }
                }
            };
        }
    }
}