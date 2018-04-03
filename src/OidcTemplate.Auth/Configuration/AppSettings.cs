namespace OpenSoftware.OidcTemplate.Auth.Configuration
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }
    }

    public class ConnectionStrings
    {
        public string AuthContext { get; set; }  
    }
}