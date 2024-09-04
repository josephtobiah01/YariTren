using Core;
using Core.Interfaces;
using Core.Models;
using Data.SharePoint.Authentication;
using Data.SharePoint.Clients;
using Microsoft.Identity.Client;
using MonkeyCache.FileStore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedLogic
{
    public class ConfigManager
    {
        //private readonly IConfigurationService _configurationClient;
        private static object _lock = new object();
        private static string _configCategory = "MobileApp";
        private IConfigurationService _configService;

        public ConfigManager(IConfigurationService configService)
        {
            _configService = configService;   
        }

        public List<ConfigItem> LoadConfig()
        {
            var config = Barrel.Current.Get<List<ConfigItem>>(Consts.ConfigDataKey);
            if (config == null)
            {
                lock (_lock)
                {
                    config = Barrel.Current.Get<List<ConfigItem>>(Consts.ConfigDataKey);
                    if (config == null)
                    {
                        config = await _configService.GetConfigByCategory(_configCategory);
                        Barrel.Current.Add(key: Consts.ConfigDataKey, data: config, expireIn: TimeSpan.FromMinutes(10)); // cache the settings for 10 minutes
                        return config;
                    }
                }

                config = ConfigurationService.GetConfigByCategory(_configCategory).Result; // Async call managed synchronously for simplicity
                Barrel.Current.Add(key: Consts.ConfigDataKey, data: config, expireIn: TimeSpan.FromMinutes(10)); // Cache the settings
            }
            //if (config == null) return new List<ConfigItem>();
            //return config;
            return config ?? new List<ConfigItem>();
        }

        public static string GetConfigValue(List<ConfigItem> config, string key, string defaultValue)
        {
            if (config == null || config.Count == 0) return defaultValue;
            if (string.IsNullOrEmpty(key)) return defaultValue;
            var configItem = config.FirstOrDefault(x => x.ConfigKey == key);
            if (configItem == null) return defaultValue;
            return configItem.ConfigValue;
        }

    }

}
