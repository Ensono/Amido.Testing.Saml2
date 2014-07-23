using System.Configuration;

namespace Amido.Testing.Saml2.TokenGenerator.Configuration
{
    public class ClaimsConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("claims", IsRequired = false)]
        [ConfigurationCollection(typeof(ClaimsConfigurationCollection), AddItemName = "claim")]
        public ClaimsConfigurationCollection Claims
        {
            get
            {
                return this["claims"] as ClaimsConfigurationCollection;
            }
        }
    }

}