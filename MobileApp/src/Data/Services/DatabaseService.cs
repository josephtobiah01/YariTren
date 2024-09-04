using System;
using SQLite;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Data.Database;
using Core.Models;
using Core.Interfaces;
using Core.Database;
using MonkeyCache.FileStore;
using System.Drawing;

namespace Data.Services
{
    public class DatabaseService : IDatabaseService
    {
        private readonly ISQLiteFactory _sQLiteFactory;
        SQLiteAsyncConnection conn;
        private const string AuthenticatedUserCacheKey = "AuthenticatedUser";

        public DatabaseService(
            ISQLiteFactory sQLiteFactory)
        {
            _sQLiteFactory = sQLiteFactory;

            InitializeConnection();
            
        }

        private void InitializeConnection()
        {
            if (conn == null)
            {
                conn = _sQLiteFactory.GetConnectionWithLock();
            }
        }

        public bool HasDatabase ()
        {
            return _sQLiteFactory.DatabaseExists ();
        }

        public async Task CloseClear ()
        {
            try 
            {
                if (conn != null)
                {
                    await conn.CloseAsync();
                }
                    
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"Error closing the database: {ex.Message}");
            }
        }

        public bool TrySetupConnection ()
        {
            try 
            {
                InitializeConnection();
            } 
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting up the database connection: {ex.Message}");
                return false;
            }
            return true;
        }

        public void ResetDB ()
        {
            _sQLiteFactory.EmptyDatabasesDir();
        }

        public async Task<AuthenticatedUser> GetUser ()
        {
            try
            {
                InitializeConnection(); 

                var cachedUser = Barrel.Current.Get<AuthenticatedUser>(AuthenticatedUserCacheKey);
                if (cachedUser != null && !Barrel.Current.IsExpired(AuthenticatedUserCacheKey))
                {
                    return cachedUser;
                }
                Console.WriteLine("Getting user from the database...");
                var user = await conn.Table<AuthenticatedUser>().FirstOrDefaultAsync();
                
                if (user != null)
                {
                    Barrel.Current.Add(AuthenticatedUserCacheKey, user, TimeSpan.FromDays(1));
                    Console.WriteLine("User cached.");
                    return user;
                }
                Console.WriteLine(user != null ? "User found." : "User not found.");
                return new AuthenticatedUser();
            }
            catch(Exception ex) 
            { 
                Console.WriteLine("Error: ", $"{ex.Message}");
                return null;
            }
        }

        public async Task InsertUpdate (AuthenticatedUser localUser)
        {
            await Task.Run (async () => 
            {
                try
                {
                    TextBlobOperations.UpdateTextBlobs(localUser);
                    await conn.InsertOrReplaceAsync(localUser);
                    // Update the cache after inserting or updating
                    Barrel.Current.Add(AuthenticatedUserCacheKey, localUser, TimeSpan.FromDays(1));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserting/updating user: {ex.Message}");
                }
               
            });
        }

        public async Task<DeviceInfo> GetDevice()
        {
            try
            {
                InitializeConnection(); // Ensure the connection is valid

                var deviceInfo = await conn.Table<DeviceInfo>().FirstOrDefaultAsync();
                return deviceInfo;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error getting device info: {e.Message}");
                return null;
            }
        }

        public async Task InsertUpdate(DeviceInfo deviceInfo)
        {
            await Task.Run(async () =>
            {
                try
                {
                    TextBlobOperations.UpdateTextBlobs(deviceInfo);
                    await conn.InsertOrReplaceAsync(deviceInfo);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inserting/updating device info: {ex.Message}");
                }
            });
        }

        public async Task ClearTables()
        {
            try
            {
                await conn.DropTableAsync<AuthenticatedUser>();
                await conn.CreateTableAsync<AuthenticatedUser>();
                // Clear the cache when the table is cleared
                Barrel.Current.Empty(AuthenticatedUserCacheKey);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing tables: {ex.Message}");
            }
        }

        public async Task<string> GetKey()
        {
            var deviceInfo = await GetDevice();
            if (deviceInfo == null)
            {
                deviceInfo = new DeviceInfo()
                {
                    Id = 1,
                    GuidText = Guid.NewGuid().ToString()
                };
                await InsertUpdate(deviceInfo);
            }
            else if (deviceInfo != null && string.IsNullOrEmpty(deviceInfo.GuidText))
            {
                deviceInfo.GuidText = Guid.NewGuid().ToString();
                await InsertUpdate(deviceInfo);
            }
            return deviceInfo.GuidText;
        }
    }
}

