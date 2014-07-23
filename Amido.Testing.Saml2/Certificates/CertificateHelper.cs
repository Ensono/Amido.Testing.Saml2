using System;
using System.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using System.Text;

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
                if (certificate.Thumbprint.ToLower() == thumbprint.ToLower())
                {
                    certificateStore.Close();
                    return certificate;
                }
            }

            throw new ArgumentException("Cannot find certificate");
        }

        public static X509SigningCredentials CreateCredentials(X509Certificate2 certificate)
        {
            return new X509SigningCredentials(certificate);
        }

        public static X509Certificate2 FromFile(string path, string password)
        {
            var cert = new X509Certificate2();

            cert.Import(path, password, X509KeyStorageFlags.DefaultKeySet);

            return cert;
        }
    }
}