using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAlertsService
    {
        /// <summary>
        /// Retrieves all alert items from the specified list with the default query options.
        /// </summary>
        /// <returns>A list of NewsItem objects containing all alerts.</returns>
        Task<List<NewsItem>> GetAllAlertItems();

        /// <summary>
        /// Retrieves alert items that are new until the current date and time, dynamically generating the filter based on the current DateTime.
        /// </summary>
        /// <returns>A list of NewsItem objects that are new until the current date and time.</returns>
        Task<List<NewsItem>> GetRecentAlertItems();
    }
}
