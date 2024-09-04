using System;
using Core.Database;
using Core.Models;
using Core.Helpers;

namespace Data.Database
{
    public class LocalDatabaseSetup
    {
        public static void CreateTables(ISQLiteFactory iSQLiteFactory)
        {
            var conn = iSQLiteFactory.GetConnectionWithLock();
            try
            {
                AsyncHelper.RunSync(async () =>
                {
                    System.Diagnostics.Debug.WriteLine("Setting up database tables");
                    await conn.CreateTableAsync<AuthenticatedUser>();
                    await conn.CreateTableAsync<DeviceInfo>();
                    System.Diagnostics.Debug.WriteLine("Database tables set up");
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
    }
}