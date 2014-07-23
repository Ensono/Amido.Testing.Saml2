using System;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;

namespace Amido.Testing.Saml2.Certificates
{
    public static class CertificateHelper
    {
        public static X509Certificate2 FindByThumbprint(string thumbprint, StoreName storeName, StoreLocation storeLocation)
        {
            var certificateStore = new X509Store(storeName, storeLocation);
            certificateStore.Open(OpenFlags.ReadWrite);

            foreach (var certificate in certificateStore.Certificates)
            {
                if (certificate == null || certificate.Thumbprint == null)
                {
                    continue;
                }

                if (String.Equals(certificate.Thumbprint, thumbprint, StringComparison.CurrentCultureIgnoreCase))
                {
                    certificateStore.Close();
                    return certificate;
                }
            }

            throw new ArgumentException(string.Format("Cannot find certificate with thumbprint {0} in certificate store ", thumbprint));
        }

        public static X509Certificate2 FindFromFile(string certificatePath, string password)
        {
            var x509Certificate2 = new X509Certificate2();

            x509Certificate2.Import(certificatePath, password, X509KeyStorageFlags.DefaultKeySet);

            return x509Certificate2;
        }

        public static X509SigningCredentials CreateSigningCredentials(X509Certificate2 certificate)
        {
            return new X509SigningCredentials(certificate);
        }
    }
}