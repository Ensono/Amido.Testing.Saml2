using System;
using System.Management.Automation;
using Amido.Testing.Saml2.Certificates;
using Amido.Testing.Saml2.Models;

namespace Amido.Testing.Saml2
{
    [Cmdlet(VerbsCommon.Get, "SamlToken")]
    public class TokenFactoryCmdLet : PSCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The trusted issuer with which the token will be created with")]
        public string Issuer { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "The trusted audience for which the token will be used with")]
        public string Audience { get; set; }

        [Parameter(Mandatory = true)]
        public string NameIdentifier { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "The thumbprint of the certificate to sign the token with")]
        public string Thumbprint { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "The store name of the certificate to sign the token with")]
        public string StoreName { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "The store location of the certificate to sign the token with")]
        public string StoreLocation { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "If true the output will be Base64 encoded")]
        public string Base64Encode { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "The start date and time of the token lifetime")]
        public string NotBeforeDate { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "The end date and time of the token lifetime")]
        public string NotOnOrAfterDate { get; set; }

        protected override void ProcessRecord()
        {
            try
            {
                var certificate = CertificateHelper.FindByThumbprint(Thumbprint, CertificateHelper.GetStoreName(StoreName), CertificateHelper.GetStoreLocation(StoreLocation));

                var saml2TokenProperties = new Saml2TokenProperties
                {
                    NameIdentifier = NameIdentifier,
                    Audience = Audience,
                    Issuer = Issuer,
                    X509Certificate2 = certificate,
                    NotBeforeDate = GetNotBeforeDate(),
                    NotOnOrAfterDate = GetNotOnOrAfterDate(),
                };

                var samlToken = new TokenFactory().CreateSaml2BearerToken(saml2TokenProperties, ParseBool(Base64Encode));

                WriteObject(samlToken);

                //TODO: Write to Clipboard

                base.ProcessRecord();
            }
            catch (Exception exception)
            {
                WriteError(new ErrorRecord(exception, "1", ErrorCategory.InvalidOperation, string.Format("An exception has been thrown generating the SAML token")));
            }
        }

        private DateTime GetNotBeforeDate()
        {
            if (string.IsNullOrEmpty(NotBeforeDate) || !ParseDateTime(NotBeforeDate))
            {
                return DateTime.UtcNow.AddHours(-24);
            }

            return Convert.ToDateTime(NotBeforeDate);
        }

        private DateTime GetNotOnOrAfterDate()
        {
            if (string.IsNullOrEmpty(NotOnOrAfterDate) || !ParseDateTime(NotOnOrAfterDate))
            {
                return DateTime.UtcNow.AddHours(24);
            }

            return Convert.ToDateTime(NotOnOrAfterDate);
        }

        private bool ParseDateTime(string dateTime)
        {
            DateTime parseDateTime;

            if (DateTime.TryParse(dateTime, out parseDateTime))
            {
                return true;
            }

            return false;
        }

        private bool ParseBool(string value)
        {
            Boolean result;

            if (Boolean.TryParse(value, out result))
            {
                return result;
            }

            return false;
        }
    }
}
