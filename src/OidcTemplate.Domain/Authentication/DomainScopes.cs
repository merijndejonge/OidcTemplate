namespace OpenSoftware.OidcTemplate.Domain.Authentication
{
    public class DomainScopes
    {
        public const string Roles = "roles";
        /// <summary>
        /// Any one who is using the MVC client application.
        /// </summary>
        public const string MvcClientUser = "mvc_client_user";
        /// <summary>
        /// Scope for api keys.
        /// </summary>
        public static string ApiKeys { get; set; }
    }
}