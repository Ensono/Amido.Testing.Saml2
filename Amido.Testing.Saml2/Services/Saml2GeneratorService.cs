using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Text;
using System.Xml;
using Amido.Testing.Saml2.Certificates;
using Amido.Testing.Saml2.Models;

namespace Amido.Testing.Saml2.Services
{
    /// <summary>
    /// SAML 2.0 Generator Service
    /// </summary>
    public class Saml2GeneratorService
    {
        /// <summary>
        /// Creates a saml2 token with a bearer subject confirmation.
        /// </summary>
        /// <param name="saml2TokenProperties">The saml2 token properties.</param>
        /// <returns>Returns a <see cref="Saml2SecurityToken"/>.</returns>
        public Saml2SecurityToken CreateSaml2TokenWithBearerSubjectConfirmation(Saml2TokenProperties saml2TokenProperties)
        {
            var saml2Conditions = new Saml2Conditions
            {
                NotBefore = saml2TokenProperties.NotBeforeDate,
                NotOnOrAfter = saml2TokenProperties.NotOnOrAfterDate
            };

            saml2Conditions.AudienceRestrictions.Add(new Saml2AudienceRestriction(new Uri(saml2TokenProperties.Audience, UriKind.RelativeOrAbsolute)));

            var saml2Assertion = new Saml2Assertion(new Saml2NameIdentifier(saml2TokenProperties.NameIdentifier))
            {
                Conditions = saml2Conditions, 
                Subject = CreateSaml2Subject(saml2TokenProperties), 
                Issuer = new Saml2NameIdentifier(saml2TokenProperties.Issuer), 
                SigningCredentials = CertificateHelper.CreateSigningCredentials(saml2TokenProperties.X509Certificate2)
            };

            AddClaims(saml2Assertion, saml2TokenProperties.Claims);

            return new Saml2SecurityToken(saml2Assertion); 
        }

        /// <summary>
        /// Gets a SAML 2.0 token string.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>A SAML 2.0 token string.</returns>
        public string GetSaml2TokenString(Saml2SecurityToken token)
        {
            var xmlWriterSettings = new XmlWriterSettings();
            var stringBuilder = new StringBuilder();

            xmlWriterSettings.OmitXmlDeclaration = true;

            using (var xmlWriter = XmlWriter.Create(stringBuilder, xmlWriterSettings))
            {
                new Saml2SecurityTokenHandler().WriteToken(xmlWriter, token);

                return stringBuilder.ToString();
            }
        }

        private static void AddClaims(Saml2Assertion saml2Assertion, Dictionary<string, string> claims)
        {
            if (claims.Count <= 0)
            {
                return;
            }

            var saml2AttributeStatement = new Saml2AttributeStatement();

            foreach (var claim in claims)
            {
                saml2AttributeStatement.Attributes.Add(new Saml2Attribute(claim.Key, claim.Value));
            }

            saml2Assertion.Statements.Add(saml2AttributeStatement);
        }

        private static Saml2Subject CreateSaml2Subject(Saml2TokenProperties saml2TokenProperties)
        {
            var subject = new Saml2Subject();
            subject.SubjectConfirmations.Add(new Saml2SubjectConfirmation(new Uri(saml2TokenProperties.SamlSubjectConfirmation)));
            subject.NameId = new Saml2NameIdentifier(saml2TokenProperties.NameIdentifier);
            return subject;
        }
    }
}