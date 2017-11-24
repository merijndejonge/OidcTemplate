using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenSoftware.OidcTemplate.Domain.Entities;

namespace OpenSoftware.OidcTemplate.Data
{
    public class IdentityContext : IdentityDbContext<ApplicationUser>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options): base(options)
        {
            
        }
    }
}
