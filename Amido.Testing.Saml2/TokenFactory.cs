using System;
using System.Text;
using Amido.Testing.Saml2.Models;
using Amido.Testing.Saml2.Services;

namespace Amido.Testing.Saml2
{
    /// <summary>
    /// FActory for creating SAML 2.0 Bearer tokens.
    /// </summary>
    public class TokenFactory
    {
        private readonly Saml2GeneratorService saml2GeneratorService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenFactory"/> class.
        /// </summary>
        public TokenFactory()
        {
            saml2GeneratorService = new Saml2GeneratorService();
        }

        /// <summary>
        /// Creates the saml2 bearer token.
        /// </summary>
        /// <param name="saml2TokenProperties">The SAML 2.0 token properties.</param>
        /// <param name="base64Encode">Base 64 encode the Bearer token string: default to true.</param>
        /// <returns>A string bearer token generated from the SAML 2.0 token.</returns>
        public string CreateSaml2BearerToken(Saml2TokenProperties saml2TokenProperties, bool base64Encode = true)
        {
            var saml2Token = saml2GeneratorService.CreateSaml2TokenWithBearerSubjectConfirmation(saml2TokenProperties);

            if (base64Encode)
            {
                return Base64Encode(saml2GeneratorService.GetSaml2TokenString(saml2Token));
            }

            return saml2GeneratorService.GetSaml2TokenString(saml2Token);
        }

        private static string Base64Encode(string token)
        {
            var encoding = Encoding.GetEncoding("iso-8859-1");
            return Convert.ToBase64String(encoding.GetBytes(token));
        }
    }
}