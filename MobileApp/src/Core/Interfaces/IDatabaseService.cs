using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Core.Models;

namespace Core.Interfaces
{
    public interface IDatabaseService
    {
        void ResetDB();
        Task<AuthenticatedUser> GetUser();
        Task InsertUpdate(AuthenticatedUser localUser);
        bool TrySetupConnection();
        Task<DeviceInfo> GetDevice();
        Task InsertUpdate(DeviceInfo localUser);
        Task CloseClear();
        bool HasDatabase();
        Task ClearTables();
        Task<string> GetKey();
    }
}

