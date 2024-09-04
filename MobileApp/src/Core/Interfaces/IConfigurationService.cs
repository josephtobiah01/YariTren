using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IConfigurationService
    {
        Task<List<ConfigItem>> GetConfigByCategory(string category);
        Task<string> GetConfigValue(string key, string defaultValue = "");
    }
}
