using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Core.Database
{
    public interface ILocalDatabase
    {
        Task<List<T>> GetAll<T>() where T : new();

        Task CreateTable<T>() where T : new();

        Task<T> QuerySingle<T>(Expression<Func<T, bool>> query) where T : new();

        Task<List<T>> QueryList<T>(Expression<Func<T, bool>> query) where T : new();

        Task<List<T>> QueryList<T>(string query, params object[] args) where T : new();

        Task<int> InsertOrReplaceAllAsync(IEnumerable items);

        Task<int> InsertOrReplaceAsync(object item);

        Task<int> Delete<T>(object primaryKey) where T : new();

        Task ClearTable<T>() where T : new();

        Task ResetData();
    }
}
