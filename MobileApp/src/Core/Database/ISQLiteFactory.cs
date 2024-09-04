using System;
using SQLite;

namespace Core.Database
{
    public interface ISQLiteFactory
    {
        SQLiteAsyncConnection GetConnectionWithLock();
        string GetSqliteDbPath();
        bool DatabaseExists();
        void EmptyDatabasesDir();
    }
}
