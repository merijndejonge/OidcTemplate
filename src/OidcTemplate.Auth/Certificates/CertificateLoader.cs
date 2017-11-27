using System.Security.Cryptography.X509Certificates;

namespace OpenSoftware.OidcTemplate.Auth.Certificates
{
    public static class CertificateLoader
    {
        /// <summary>
        /// Loads certificate from store or disk
        /// </summary>
        /// <param name="thumbPrint">The thumpPrint of the certificate when loading from store</param>
        /// <param name="pathToCertificate">Path on disk to certificate file</param>
        /// <param name="password">Password of certificate</param>
        /// <param name="isTrusted">True when certificate is trusted, fale otherwise</param>
        /// <returns></returns>
        public static X509Certificate2 Load(string thumbPrint, string pathToCertificate, string password, bool isTrusted)
        {
            X509Certificate2 cert = null;
            using (var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                certStore.Open(OpenFlags.ReadOnly);
                var certCollection = certStore.Certificates.Find(
                    X509FindType.FindByThumbprint, thumbPrint, isTrusted);
                // Get the first cert with the thumprint
                if (certCollection.Count > 0)
                {
                    // Successfully loaded cert from registry
                    cert = certCollection[0];
                }
            }
            // Fallback to local file for development
            return cert ?? new X509Certificate2(pathToCertificate, password);
        }
    }
}