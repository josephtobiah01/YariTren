using Core.Interfaces;
using Core.Models;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.SharePoint.Clients
{
    public class NewsService : INewsService
    {
        private readonly IClientManager _clientManager;
        private readonly string _listName = "News";
        //private readonly string _selectFields = "Id,Title,ManagementLevel,Function,UserAudience,LocationAudience";
        //private readonly string _baseQuery;

        public NewsService(
            IClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        public async Task<List<NewsItem>> GetAllNewsItems()
        {
            string oDataQuery = $"?$select=Id,Title,ManagementLevel,Function,UserAudience,LocationAudience";
            return await _clientManager.GetAllItems<NewsItem>(_listName, oDataQuery);
        }

        public async Task<List<NewsItem>> GetRecentNewsItems()
        {
            string filterForCurrentDate = $"$filter=NewUntil gt datetime'{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}'";
            string oDataQuery = $"?$select=Id,Title,ManagementLevel,Function,UserAudience,LocationAudience&{filterForCurrentDate}";
            return await _clientManager.GetAllItems<NewsItem>(_listName, oDataQuery);
        }
    }
}
