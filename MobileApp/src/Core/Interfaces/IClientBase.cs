using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IClientBase
    {
        Task<IList<object>> GetAll(string odataQuery = null);
        Task<IList<object>> GetSingleItem();
        Task<List<object>> GetItemByCustomQuery(string customUrl);
        Task<IList<object>> GetSingleItemForUpdate();
        Task<int> GetCount();
        Task<string> GetListItemEntityTypeFullName();
        Task<bool> DownloadFile(string url, string saveLocation);
    }
}
