using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;
using OpenSoftware.OidcTemplate.Data;
using OpenSoftware.OidcTemplate.Domain.Authentication;
using OpenSoftware.OidcTemplate.Domain.Entities;

namespace OpenSoftware.OidcTemplate.Auth.Configuration
{
    /// <inheritdoc />
    /// <summary>
    /// AspNet profile service
    /// </summary>
    public class ProfileService : IdentityServer4.AspNetIdentity.ProfileService<ApplicationUser>
    {
        private readonly IdentityContext _context;

        public ProfileService(UserManager<ApplicationUser> userManager,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory, IdentityContext context) : base(userManager,
            claimsFactory)
        {
            _context = context;
        }

        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            await base.GetProfileDataAsync(context);

            var userData = _context.Users.FirstOrDefault(x => x.Id == context.Subject.FindFirstValue(JwtClaimTypes.Subject));
            Debug.Assert(userData != null, nameof(userData) + " != null");

            var userName = userData.FirstName.IsNullOrEmpty() || userData.LastName.IsNullOrEmpty()
                ? userData.Email
                : $"{userData.FirstName} {userData.LastName}";

            var userClaims = new List<Claim>
            {
                new Claim(DomainClaimTypes.TestUserId, "some test_user_id"),
                new Claim(DomainClaimTypes.LiveUserId, "some live_user_id"),
                new Claim(DomainClaimTypes.LiveEnabled, "true"),
                new Claim(DomainClaimTypes.SomeClaim, "some_claim value"),
                new Claim(DomainClaimTypes.AnotherClaim, "another_claim value"),
                new Claim("name", userName)
            };

            context.AddRequestedClaims(userClaims);
        }
    }
}