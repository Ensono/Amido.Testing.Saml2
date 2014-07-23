using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace Amido.Testing.Saml2.Models
{
    public class Saml2TokenProperties
    {
        public string NameIdentifier { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public DateTime NotBeforeDate { get; set; } 
        public DateTime NotOnOrAfterDate { get; set; }
        public X509Certificate2 X509Certificate2 { get; set; }
        public Dictionary<string, string> Claims { get; set; }

        public Saml2TokenProperties()
        {
            Claims = new Dictionary<string, string>();
        }
    }
}