using System;
using System.Management.Automation;
using System.Windows.Forms;
using Amido.Testing.Saml2.Certificates;
using Amido.Testing.Saml2.Models;

namespace Amido.Testing.Saml2
{
    [Cmdlet(VerbsCommon.Get, "SamlToken")]
    public class TokenFactoryCmdLet : PSCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The trusted issuer with which the token will be created with")]
        [Alias("i")]
        public string Issuer { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "The trusted audience for which the token will be used with")]
        [Alias("a")]
        public string Audience { get; set; }

        [Parameter(Mandatory = true)]
        [Alias("id")]
        public string NameIdentifier { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "The thumbprint of the certificate to sign the token with")]
        [Alias("t")]
        public string Thumbprint { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "The store name of the certificate to sign the token with")]
        public string StoreName { get; set; }

        [Parameter(Mandatory = true, HelpMessage = "The store location of the certificate to sign the token with")]
        public string StoreLocation { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "If true the output will be Base64 encoded.  Default is true")]
        public string Base64Encode { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "If true the output will be Url encoded.  Default is false")]
        public string UrlEncode { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "The start date and time of the token lifetime.  Default is 24 hours in the past")]
        public string NotBeforeDate { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "The end date and time of the token lifetime.  Default is 24 hours in the future")]
        public string NotOnOrAfterDate { get; set; }

        [Parameter(Mandatory = false, HelpMessage = "The type of token to be used.  Default is bearer token")]
        public string SamlSubjectConfirmation { get; set; }

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
                    SamlSubjectConfirmation = GetSamlSubjectConfirmation()
                };

                var samlToken = new TokenFactory().CreateSaml2Token(saml2TokenProperties, ParseBool(Base64Encode, true), ParseBool(UrlEncode, false));

                WriteObject(samlToken);

                Clipboard.SetText(samlToken);

                WriteVerbose("Saml Token generated and added to the clipboard");

                base.ProcessRecord();
            }
            catch (Exception exception)
            {
                WriteError(new ErrorRecord(exception, "1", ErrorCategory.InvalidOperation, string.Format("An exception has been thrown generating the SAML token")));
            }
        }

        private string GetSamlSubjectConfirmation()
        {
            if (string.IsNullOrEmpty(SamlSubjectConfirmation))
            {
                return "urn:oasis:names:tc:SAML:2.0:cm:bearer";
            }

            return SamlSubjectConfirmation;
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

        private bool ParseBool(string value, bool defaultValue)
        {
            Boolean result;

            if (Boolean.TryParse(value, out result))
            {
                return result;
            }

            return defaultValue;
        }
    }
}
