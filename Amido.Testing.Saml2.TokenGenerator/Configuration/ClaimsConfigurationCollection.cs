using System.Configuration;

namespace Amido.Testing.Saml2.TokenGenerator.Configuration
{
    public class ClaimsConfigurationCollection : ConfigurationElementCollection
    {
        public new ClaimConfigurationElement this[string key]
        {
            get
            {
                return this.BaseGet(key) as ClaimConfigurationElement;
            }
        }

        public ClaimConfigurationElement this[int index]
        {
            get
            {
                return this.BaseGet(index) as ClaimConfigurationElement;
            }
            set
            {
                if (this.BaseGet(index) != null)
                    this.BaseRemoveAt(index);
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ClaimConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ClaimConfigurationElement)element).Name;
        }
    }
}