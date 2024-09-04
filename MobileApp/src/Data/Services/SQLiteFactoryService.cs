using Core.Database;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Storage;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Services
{
    public class SQLiteFactoryService : ISQLiteFactory
    {
        private const string DatabaseFilename = "YarraLocalStore.db";
        private static readonly object Locker = new object();
        //private static readonly string DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DatabaseFilename);

        ////#if ANDROID
        //if(Microsoft.Maui.Devices.DeviceInfo.)
        //        private static readonly string DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), DatabaseFilename);
        //#elif IOS
        //        private static readonly string DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", DatabaseFilename);
        //#else
        //        private static readonly string DatabasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DatabaseFilename);
        //#endif
        private static string DatabasePath = Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        public SQLiteFactoryService()
        {
        }

        public SQLiteAsyncConnection GetConnectionWithLock()
        {
            //return new SQLiteAsyncConnection(GetSqliteDbPath());

            lock (Locker)
            {
                var connection = new SQLiteAsyncConnection(DatabasePath);
                return connection;
            }
        }

        public string GetSqliteDbPath()
        {
            return DatabasePath;
        }

        public bool DatabaseExists()
        {
            return File.Exists(DatabasePath);
        }

        public void EmptyDatabasesDir()
        {
            if (DatabaseExists())
            {
                File.Delete(DatabasePath);
            }
        }

    }
}
