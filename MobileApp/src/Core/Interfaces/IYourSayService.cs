using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IYourSayService
    {
        Task<bool> SaveYourSay(YourSay yourSayData);
        Task<bool> SaveYourSayAttachment(YourSay yourSay);
        List<string> GetTravelDirections();
        List<Route> LoadJourneyRoutes();
    }
}
