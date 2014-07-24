using System;
using System.Text;
using System.Web;
using Amido.Testing.Saml2.Models;
using Amido.Testing.Saml2.Services;

namespace Amido.Testing.Saml2
{
    /// <summary>
    /// Factory for creating SAML 2.0 Bearer tokens.
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
        /// Creates the Saml2 token.
        /// </summary>
        /// <param name="saml2TokenProperties">The SAML 2.0 token properties.</param>
        /// <param name="base64Encode">Base 64 encode the token string: default to true.</param>
        /// <param name="urlEncode">Url Encode the token string: default to false</param>
        /// <returns>A string token generated from the SAML 2.0 token.</returns>
        public string CreateSaml2Token(Saml2TokenProperties saml2TokenProperties, bool base64Encode = true, bool urlEncode = false)
        {
            var saml2Token = saml2GeneratorService.CreateSaml2TokenWithBearerSubjectConfirmation(saml2TokenProperties);

            var saml2TokenString = saml2GeneratorService.GetSaml2TokenString(saml2Token);

            if (base64Encode)
            {
                saml2TokenString = Base64Encode(saml2TokenString);
            }

            if (urlEncode)
            {
                saml2TokenString = HttpUtility.UrlEncode(saml2TokenString);
            }

            return saml2TokenString;
        }

        private static string Base64Encode(string token)
        {
            var encoding = Encoding.GetEncoding("iso-8859-1");
            return Convert.ToBase64String(encoding.GetBytes(token));
        }
    }
}