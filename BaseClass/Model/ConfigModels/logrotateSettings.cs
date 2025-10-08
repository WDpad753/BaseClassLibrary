using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Model.ConfigModels
{
    public class logrotateSettings : ConfigurationSection
    {
        /// <summary>
        /// The name of this section in the app.config.
        /// </summary>
        public const string SectionName = "logrotateSettings";

        private const string CollectionName = "settings";

        [ConfigurationProperty(CollectionName)]
        [ConfigurationCollection(typeof(logrotateSettingsCollection), AddItemName = "add")]
        public logrotateSettingsCollection LogRotateSettings
        {
            get
            {
                return (logrotateSettingsCollection)base[CollectionName];
            }
        }
    }

    public class logrotateSettingsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new logrotateSettingElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((logrotateSettingElement)element).key;
        }

        public logrotateSettingElement this[string key]
        {
            get { return (logrotateSettingElement)BaseGet(key); }
        }
    }

    public class logrotateSettingElement : ConfigurationElement
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