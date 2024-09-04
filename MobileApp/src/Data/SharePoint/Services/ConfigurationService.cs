using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MonkeyCache.FileStore;
using System.Web;
using Core;
using System.Collections;

namespace Data.SharePoint.Clients
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IClientManager _clientManager;
        private readonly string _listName = "Configuration";
        private readonly string _cacheKey = "ConfigurationItemsCache";
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(10);

        public string ODataQuery { get; set; }
        public string SelectFields { get; set; }

        public ConfigurationService(IClientManager clientManager)
        {
            _clientManager = clientManager;
            ODataQuery = $"?$select={SelectFields}";
        }

        public async Task<List<ConfigItem>> GetConfigByCategory(string category)
        {
            var config = Barrel.Current.Get<List<ConfigItem>>(Consts.ConfigDataKey);
            if (config == null)
            {
                config = await GetConfigItemsFromSharePointByCategory(category);
                if (config != null)
                {
                    Barrel.Current.Add(key: Consts.ConfigDataKey, data: config, expireIn: TimeSpan.FromMinutes(10)); // cache the settings for 10 minutes
                    return config;
                }
            }
            return config ?? new List<ConfigItem>();
        }

        public async Task<string> GetConfigValue(string key, string defaultValue = "")
        {
            var configItems = await GetConfigByCategory(string.Empty);
            var configItem = configItems?.Find(item => item.ConfigKey == key);
            return configItem?.ConfigValue ?? defaultValue;
        }

        private async Task<List<ConfigItem>> GetConfigItemsFromSharePointByCategory(string category)
        {
            string query = $"?$select=Id,Title,ConfigValue";
            if (!string.IsNullOrEmpty(category))
            {
                query = $"?$select=Id,Title,ConfigValue&$filter=ConfigCategory eq '{category}'";
            }
            return await _clientManager.GetAllItems<ConfigItem>(_listName, query);
        }

    }
}
