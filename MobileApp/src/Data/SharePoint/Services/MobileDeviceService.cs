using Core;
using Core.Interfaces;
using Core.Models;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices;
using MonkeyCache.FileStore;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using DeviceInfo = Microsoft.Maui.Devices.DeviceInfo;

namespace Data.SharePoint.Services
{
    public class MobileDeviceService : IMobileDeviceService
    {
        private readonly IDatabaseService _databaseService;
        //private readonly ICustomService _customService;
        private readonly RestClient _client;
        private readonly string _siteUrl = Consts.SiteUrl;
        private readonly IYourSayService _yourSayService;
        private readonly IConfigurationService _configService;
        private DateTime? LastUnlockTime;
        private const int AppLockTimeout = 10;
        private static string _configCategory = "MobileApp";

        public MobileDeviceService(
            IDatabaseService databaseService,
            //ICustomService customService,
            IYourSayService yourSayService,
            IConfigurationService configService)
        {
            _databaseService = databaseService;
            //_customService = customService;
            _yourSayService = yourSayService;
            _configService = configService;

            _client = new RestClient();
        }

        public string GetCurrentVersion()
        {
            return $"{VersionTracking.CurrentVersion}";
        }

        
        private void SetTags(UserDevice userDevice)
        {
            var tags = new List<string>();
            tags.AddRange(userDevice.LocationAudience.Where(val => !string.IsNullOrEmpty(val)).Select(val => $"{val};"));
            tags.AddRange(userDevice.UserAudience.Where(val => !string.IsNullOrEmpty(val)).Select(val => $"{val};"));
            tags.AddRange(userDevice.ManagementLevel.Where(val => !string.IsNullOrEmpty(val)).Select(val => $"{val};"));
            tags.AddRange(userDevice.Function.Where(val => !string.IsNullOrEmpty(val)).Select(val => $"{val};"));

            userDevice.Tags = string.Join("", tags);
        }

        public UserDevice GetDeviceInfo()
        {
            VersionTracking.Track();
            UserDevice userDevice = new UserDevice
            {
                AppVersion = VersionTracking.CurrentVersion,
                DevicePlatform = Microsoft.Maui.Devices.DeviceInfo.Platform.ToString(),
                DeviceOSVersion = Microsoft.Maui.Devices.DeviceInfo.VersionString,
                DeviceType = Microsoft.Maui.Devices.DeviceInfo.Name
            };
            return userDevice;
        }

        public async Task<UserDevice> CreateOrUpdateListItem(UserDevice userDevice)
        {

            string listUrl = $"{_siteUrl}/_api/web/lists/GetByTitle('User Devices')/items";
            var method = userDevice.Id > 0 ? Method.Post : Method.Put;
            var request = new RestRequest(listUrl, method);

            request.AddJsonBody(new
            {
                __metadata = new { type = "SP.UserDeviceListItem" },
                userDevice.DeviceInstallationId,
                userDevice.DeviceOS,
                userDevice.DeviceOSVersion,
                userDevice.DevicePushToken,
                userDevice.DeviceType,
                userDevice.EmployeeID,
                userDevice.EmployeeName,
                userDevice.Office
            });

            if (userDevice.Id > 0)
            {
                request.AddHeader("X-HTTP-Method", "MERGE");
                request.AddHeader("IF-MATCH", "*"); // Handle concurrency issues
            }

            var response = await _client.ExecuteAsync<UserDevice>(request);
            return response.IsSuccessful ? JsonConvert.DeserializeObject<UserDevice>(response.Content) : null;
        }

        public async void SetUserDeviceInfo(UserDevice userDevice, UserProfile userProfile)
        {
            userDevice.EmployeeName = userProfile.DisplayName;
            userDevice.EmployeeID = userProfile.EmployeeNumber;
            userDevice.Office = userProfile.Office;
            userDevice.LocationAudience = new List<string> { userProfile.Office };
            userDevice.UserAudience = new List<string> { userProfile.EmployeeRole };
            userDevice.ManagementLevel = new List<string> { userProfile.ManagementLevel };
            userDevice.Function = new List<string> { userProfile.Function };

            SetTags(userDevice);
            await CreateOrUpdateListItem(userDevice);
        }

        public async Task SendUserDeviceInfoAsync(UserProfile userProfile)
        {
            try
            {
                var deviceInfo = await _databaseService.GetDevice();
                if (deviceInfo != null && !string.IsNullOrEmpty(deviceInfo.DeviceTokenText))
                {
                    UserDevice userDevice = GetDeviceInfo();
                    userDevice.DevicePushToken = deviceInfo.DeviceTokenText;
                    userDevice.DeviceInstallationId = deviceInfo.GuidText;
                    SetUserDeviceInfo(userDevice, userProfile);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error sending user device info: {ex.Message}");
            }
        }

        public void RunBackgroundTasks(UserProfile userProfile)
        {
            var lastRunKey = "backgroundTasksLastRun";
            DateTime? lastRun = Barrel.Current.Get<DateTime>(lastRunKey);
            var fiveMinutesAgo = DateTime.Now.AddMinutes(-5);
            if (lastRun == null || lastRun < fiveMinutesAgo)
            {
                Task.Run(() => _yourSayService.LoadJourneyRoutes());
                Task.Run(() => SendUserDeviceInfoAsync(userProfile));
                Task.Run(() => LoadConfig());
                //if (DeviceInfo.Platform == DevicePlatform.iOS)
                //{
                //    Task.Run(() => _customService.UpdateBadgeCount());
                //}
                Barrel.Current.Add(key: lastRunKey, data: DateTime.Now, expireIn: TimeSpan.FromMinutes(5));
            }
        }

        public List<Route> LoadJourneyRoutes()
        {
            return _yourSayService.LoadJourneyRoutes();
        }

        public async Task<List<ConfigItem>> LoadConfig()
        {
            return await _configService.GetConfigByCategory(_configCategory);
        }

        public void ResetAppActivity()
        {
            LastUnlockTime = DateTime.Now;
        }

        public bool HasActivityExpired()
        {
            return LastUnlockTime.HasValue && (DateTime.Now - LastUnlockTime.Value) > new TimeSpan(0, AppLockTimeout, 0);
        }

    }
}
