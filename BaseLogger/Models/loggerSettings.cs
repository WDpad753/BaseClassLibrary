using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseLogger.Models
{
    public class loggerSettings : ConfigurationSection
    {
        /// <summary>
        /// The name of this section in the app.config.
        /// </summary>
        public const string SectionName = "loggerSettings";

        private const string CollectionName = "settings";

        [ConfigurationProperty(CollectionName)]
        [ConfigurationCollection(typeof(loggerSettingsCollection), AddItemName = "add")]
        public loggerSettingsCollection LoggerSettings 
        { 
            get 
            { 
                return (loggerSettingsCollection)base[CollectionName]; 
            } 
        }
    }

    public class loggerSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new loggerSettingElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((loggerSettingElement)element).key;
        }

        public loggerSettingElement this[string key]
        {
            get { return (loggerSettingElement)BaseGet(key); }
        }
    }

    public class loggerSettingElement : ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true)]
        public string key
        {
            get { return (string)this["key"]; }
            set { this["key"] = value; }
        }

        [ConfigurationProperty("value")]
        public string value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }
    }
}