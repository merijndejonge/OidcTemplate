using Microsoft.AspNetCore.Identity;

namespace OpenSoftware.OidcTemplate.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string ApiKey { get; set; }
    }
}