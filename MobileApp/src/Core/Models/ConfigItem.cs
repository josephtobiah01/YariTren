using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class ConfigItem
    {
        public string Title { get; set; }
        public string ConfigValue { get; set; }

        // Note: only adding this here as RestSharp does not currently support annotation for serialization.
        // The field that is used for ConfigKey comes back as 'Title' - so need this for serialization, but will 
        // use ConfigKey as the configuration key
        public string ConfigKey
        {
            get
            {
                return Title;
            }
        }
    }
}
