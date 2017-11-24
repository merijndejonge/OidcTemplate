using System.Text.Encodings.Web;
using System.Threading.Tasks;
using OpenSoftware.OidcTemplate.Auth.Services;

namespace OpenSoftware.OidcTemplate.Auth.Extensions
{
    public static class EmailSenderExtensions
    {
        public static Task SendEmailConfirmationAsync(this IEmailSender emailSender, string email, string link)
        {
            return emailSender.SendEmailAsync(email, "Confirm your email",
                $"Please confirm your account by clicking this link: <a href='{HtmlEncoder.Default.Encode(link)}'>link</a>");
        }
    }
}
