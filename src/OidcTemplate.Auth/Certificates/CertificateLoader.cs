using System.IO;
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
        public static X509Certificate2 Load(string thumbPrint, string pathToCertificate, string password,
            bool isTrusted)
        {
            return TryLoadCertFromStore(thumbPrint, isTrusted) ?? TryLoadCertFromFile(pathToCertificate, password);
        }

        private static X509Certificate2 TryLoadCertFromStore(string thumbPrint, bool isTrusted)
        {
            using (var certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                certStore.Open(OpenFlags.ReadOnly);
                var certCollection = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbPrint, isTrusted);
                // Get the first cert with the thumprint or null
                return certCollection.Count > 0 ? certCollection[0] : null;
            }
        }
        private static X509Certificate2 TryLoadCertFromFile(string pathToCertificate, string password)
        {
            if (File.Exists(pathToCertificate) == false)
            {
                throw new FileNotFoundException(pathToCertificate);
            }

            var certCollection = new X509Certificate2Collection();
            certCollection.Import(pathToCertificate, password, X509KeyStorageFlags.PersistKeySet);
            // Get the first cert from the collection or null
            return certCollection.Count > 0 ? certCollection[0] : null;
        }
    }
}