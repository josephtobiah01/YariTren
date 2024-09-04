using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface INewsService
    {
        Task<List<NewsItem>> GetAllNewsItems();
        Task<List<NewsItem>> GetRecentNewsItems();
    }
}
