using Core.Helpers;
using Core.Models;
using Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace SharedLogic
{
    public class DatabaseManager
    {
        private readonly IDatabaseService _databaseService;

        public DatabaseManager(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<AuthenticatedUser> GetUser()
        {
            //return FreshIOC.Container.Resolve<IDatabaseService>().GetUser();
            return await _databaseService.GetUser();
        }

        public async Task<DeviceInfo> GetDevice()
        {
            return await _databaseService.GetDevice();
        }
        public Task SaveDevice(DeviceInfo device)
        {
            //FreshIOC.Container.Resolve<IDatabaseService>().InsertUpdate(device);
            return _databaseService.InsertUpdate(device);
        }

        public async Task<string> GetKey()
        {
            var deviceInfo = await _databaseService.GetDevice();
            if (deviceInfo == null)
            {
                deviceInfo = new DeviceInfo()
                {
                    Id = 1,
                    GuidText = Guid.NewGuid().ToString()
                };
                await this.SaveDevice(deviceInfo);
            }
            else if(deviceInfo != null && string.IsNullOrEmpty(deviceInfo.GuidText))
            {
                deviceInfo.GuidText = Guid.NewGuid().ToString();
                await this.SaveDevice(deviceInfo);
            }
            return deviceInfo.GuidText;
        }

        public void ResetData()
        {
            AsyncHelper.RunSync(() => _databaseService.ClearTables()); /*FreshIOC.Container.Resolve<IDatabaseService>().ClearTables());*/
            
        }
    }
}
