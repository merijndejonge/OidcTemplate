using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using OpenSoftware.OidcTemplate.Data;
using OpenSoftware.OidcTemplate.Domain.Entities;

namespace OpenSoftware.OidcTemplate.Auth.Configuration
{
    /// <summary>
    /// AspNet profile service
    /// According to https://www.youtube.com/watch?v=3rtq8M1s95c at 51:49 Ben Cull says there is now a built-in one now. 
    /// todo: figure out where to find this default one
    /// </summary>
    public class ProfileService : AspNetIdentityProfileService<ApplicationUser>    {
        private readonly IdentityContext _context;

        public ProfileService(UserManager<ApplicationUser> userManager, IdentityContext context) : base(userManager)
        {
            _context = context;
        }

        protected override async Task<List<Claim>> GetIdentityClaims(ApplicationUser applicationUser)
        {
            // Grab the built in identity claims
            var claims = await base.GetIdentityClaims(applicationUser);

            var userData = _context.Users.FirstOrDefault(x => x.Id == applicationUser.Id);

            //if (userData?.Merchant?.SubMerchants.FirstOrDefault(x => !x.IsLive) == null
            //    || userData?.Merchant?.SubMerchants.FirstOrDefault(x => x.IsLive) == null)
            //{
            //    throw new NullReferenceException($"USer with id: {user.Id} does not have both a test and a live merchant.");
            //}

            var userName = userData.FirstName.IsNullOrEmpty() || userData.LastName.IsNullOrEmpty()
                ? userData.Email
                : $"{userData.FirstName} {userData.LastName}";
            var merchantClaims = new List<Claim>
            {
                new Claim("test_submerchant_id", "some test_submerchant_id"),
                new Claim("live_submerchant_id","some live_submerchant_id"),
                new Claim("live_enabled", "some live_enabled"),
                new Claim("merchant_id", "some merchant_id"),
                new Claim("merchant_name", "some merchant_name"),
                new Claim("name", userName)
            };
            claims.AddRange(merchantClaims);

            return claims;
        }
    }

    public class AspNetIdentityProfileService<TUser> : IProfileService where TUser : IdentityUser, new()
    {
        private readonly UserManager<TUser> _userManager;

        public AspNetIdentityProfileService(UserManager<TUser> userManager)
        {
            _userManager = userManager;
        }
        /// <summary>
        /// This method is called whenever claims about the user are requested (e.g. during token creation or via the userinfo endpoint
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">subject</exception>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.FindByIdAsync(context.Subject.GetSubjectId());
            if (user == null)
            {
                throw new ArgumentException($"Could not find user with the given sub: {context.Subject.GetSubjectId()}");
            }

            var claims = await GetIdentityClaims(user);
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);
            claims = claims.Where(x => context.RequestedClaimTypes.Contains(x.Type)).ToList();

            context.IssuedClaims = claims;
        }
        /// <summary>
        /// This method gets called whenever identity server needs to determine if the user is valid or active (e.g. durign tooken issue
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">subject</exception>
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var user = await _userManager.FindByIdAsync(context.Subject.GetSubjectId());
            if (user == null)
            {
                throw new ArgumentException($"Could not find user with the given sub: {context.Subject.GetSubjectId()}");
            }

            // TODO: Implement disabled users
            context.IsActive = true;
        }

        protected virtual async Task<List<Claim>>GetIdentityClaims(TUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id),
                new Claim(JwtClaimTypes.PreferredUserName, user.UserName)
            };

            if (_userManager.SupportsUserEmail)
            {
                var email = await _userManager.GetEmailAsync(user);
                if (!string.IsNullOrWhiteSpace(email))
                {
                    claims.Add(new Claim(JwtClaimTypes.Email, email));
                    var verified = await _userManager.IsEmailConfirmedAsync(user);
                    claims.Add(new Claim(JwtClaimTypes.EmailVerified, verified ? "true" : "false"));
                }
            }
            if (_userManager.SupportsUserPhoneNumber)
            {
                var phone = await _userManager.GetPhoneNumberAsync(user);
                if (!string.IsNullOrWhiteSpace(phone))
                {
                    claims.Add(new Claim(JwtClaimTypes.PhoneNumber, phone));
                    var verified = await _userManager.IsPhoneNumberConfirmedAsync(user);
                    claims.Add(new Claim(JwtClaimTypes.PhoneNumberVerified, verified ? "true" : "false"));
                }
            }
            if (_userManager.SupportsUserClaim)
            {
                claims.AddRange(await  _userManager.GetClaimsAsync(user));
            }
            if (_userManager.SupportsUserRole)
            {
                var roleClaims = from role in await _userManager.GetRolesAsync(user)
                    select new Claim(JwtClaimTypes.Role, role);
                claims.AddRange(roleClaims);
            }

            return claims;
        }
    }
}