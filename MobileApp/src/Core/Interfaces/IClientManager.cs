using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IClientManager
    {
        Task<List<T>> GetAllItems<T>(string listName, string odataQuery = null);
        Task<T> GetSingleItem<T>(string listName, int id) where T : class;
        Task<IList<T>> GetItemsByCustomUrl<T>(string customUrl);
        Task<T> GetItemByCustomUrl<T>(string customUrl);
        Task<T> CreateItem<T>(string listName, object body);
        Task<int> GetCount(string listName, string odataQuery = null);
        Task<string> GetListItemEntityTypeFullName(string listName);
        Task<bool> UploadAttachment(string listName, int itemId, byte[] attachmentBytes, string attachmentName);
        Task<bool> DownloadFile(string url, string saveLocation);
        Task EnsureClient();
    }
}
