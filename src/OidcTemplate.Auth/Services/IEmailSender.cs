using System.Threading.Tasks;

namespace OpenSoftware.OidcTemplate.Auth.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
