using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IMobileDeviceService
    {
        string GetCurrentVersion();
        UserDevice GetDeviceInfo();
        Task SendUserDeviceInfoAsync(UserProfile userProfile);
        void RunBackgroundTasks(UserProfile userProfile);
        Task<UserDevice> CreateOrUpdateListItem(UserDevice userDevice);
        void SetUserDeviceInfo(UserDevice userDevice, UserProfile userProfile);
        List<Route> LoadJourneyRoutes();
        Task<List<ConfigItem>> LoadConfig();
        void ResetAppActivity();
        bool HasActivityExpired();
    }
}
