using Core.Interfaces;
using Core.Models;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Data.SharePoint.Clients
{
    public class AlertsService : IAlertsService
    {
        private readonly IClientManager _clientManager;
        private readonly string _listName = "Alerts";
        //private readonly string _selectFields = "Id,Title,ManagementLevel,Function,UserAudience,LocationAudience";



        public AlertsService(
            IClientManager clientManager)
        {
            _clientManager = clientManager;
        }

        public async Task<List<NewsItem>> GetAllAlertItems()
        {
            string oDataQuery = $"?$select=Id,Title,ManagementLevel,Function,UserAudience,LocationAudience";
            return await _clientManager.GetAllItems<NewsItem>(_listName, oDataQuery);
        }

        public async Task<List<NewsItem>> GetRecentAlertItems()
        {
            string filterForCurrentDate = $"$filter=NewUntil gt datetime'{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}'";
            string oDataQuery = $"?$select=Id,Title,ManagementLevel,Function,UserAudience,LocationAudience&{filterForCurrentDate}";
            return await _clientManager.GetAllItems<NewsItem>(_listName, oDataQuery);
        }
    }
}
