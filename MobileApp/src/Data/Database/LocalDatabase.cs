using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq;
using SQLite;
using Core.Database;

namespace Data.Database
{
    public class LocalDatabase : ILocalDatabase
    {
        readonly SQLiteAsyncConnection _asyncConn;
        ISQLiteFactory _factory;

        public LocalDatabase(ISQLiteFactory factory)
        {
            _asyncConn = factory.GetConnectionWithLock();
            _factory = factory;
        }

        public async Task CreateTable<T>() where T : new()
        {
            try
            {
                await _asyncConn.CreateTableAsync<T>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        public async Task<T> QuerySingle<T>(Expression<Func<T, bool>> query) where T : new()
        {
            var item = await _asyncConn.Table<T>().Where(query).FirstOrDefaultAsync();
            TextBlobOperations.GetTextBlobs(item);
            return item;
        }

        public async Task<List<T>> QueryList<T>(Expression<Func<T, bool>> query) where T : new()
        {
            var list = await _asyncConn.Table<T>().Where(query).ToListAsync();
            foreach (var item in list)
            {
                TextBlobOperations.GetTextBlobs(item);
            }
            return list;
        }


        public async Task<List<T>> QueryList<T>(string query, params object[] args) where T : new()
        {
            var list = (await _asyncConn.QueryAsync<T>(query, args));
            foreach (var item in list)
            {
                TextBlobOperations.GetTextBlobs(item);
            }
            return list;
        }

        public async Task<List<T>> GetAll<T>() where T : new()
        {
            var list = (await _asyncConn.Table<T>().ToListAsync());
            foreach (var item in list)
            {
                TextBlobOperations.GetTextBlobs(item);
            }
            return list;
        }

        public async Task<int> InsertOrReplaceAllAsync(IEnumerable items)
        {
            foreach (var item in items)
            {
                TextBlobOperations.UpdateTextBlobs(item);
            }
            foreach (var item in items)
                await _asyncConn.InsertOrReplaceAsync(item);
            return 0;
        }

        public async Task<int> InsertOrReplaceAsync(object item)
        {
            TextBlobOperations.UpdateTextBlobs(item);
            var insert = await _asyncConn.InsertOrReplaceAsync(item);
            return insert;
        }

        public async Task<int> Delete<T>(object primaryKey) where T : new()
        {
            var item = await _asyncConn.GetAsync<T>(primaryKey);
            await _asyncConn.DeleteAsync(item);
            return 1;
        }

        public async Task ClearTable<T>() where T : new()
        {
            var conn = _factory.GetConnectionWithLock();
            await conn.DropTableAsync<T>();
            await conn.CreateTableAsync<T>();
        }

        public async Task ResetData()
        {
            //await ClearTable<>();
        }
    }

}
