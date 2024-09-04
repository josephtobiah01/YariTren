//using Core.Database;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTramsMobileMauiBlazor.Services
{
    public class SQLiteFactory : ISQLiteFactory
    {
        private string _databasePath;

        public SQLiteFactory()
        {
            _databasePath = Path.Combine(FileSystem.AppDataDirectory, "LocalStore.db");
        }

        public SQLiteAsyncConnection GetConnectionWithLock()
        {
            return new SQLiteAsyncConnection(_databasePath);
        }

        public string GetSqliteDbPath()
        {
            return _databasePath;
        }

        public bool DatabaseExists()
        {
            return File.Exists(_databasePath);
        }

        public void EmptyDatabasesDir()
        {
            var directory = Path.GetDirectoryName(_databasePath);
            if (directory == null) return;

            var files = Directory.GetFiles(directory);
            foreach (var file in files)
                File.Delete(file);
        }
    }
}
