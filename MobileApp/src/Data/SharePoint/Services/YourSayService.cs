using Core.Models;
using Core.Analytics;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;
using Core;
using MonkeyCache.FileStore;
using Data.SharePoint.Authentication;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Core.Helpers;

namespace Data.SharePoint.Clients
{
    public class YourSayService : IYourSayService
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly IClientManager _clientManager;
        private static object _lock = new object();
        private static string _listName = "Your Say";

        public YourSayService(IAnalyticsService analyticsService, IClientManager clientManager)
        {
            _analyticsService = analyticsService;
            _clientManager = clientManager;
        }

        public async Task<bool> SaveYourSay(YourSay yourSayData)
        {
            int retryCount = 0;
            YourSay yourSay = null;
            while (retryCount < 3)
            {
                try
                {
                    var yourSayDTO = new YourSayRestDTO(yourSayData);
                    yourSay = await _clientManager.CreateItem<YourSay>("Your Say", yourSayDTO);
                    break;
                }
                catch (Exception ex)
                {
                    string message = $"Failed to save Your Say list item, retry count: {retryCount}";
                    _analyticsService.ReportException(ex, message);
                }
                finally
                {
                    retryCount++;
                }
            }
            if (yourSay != null && yourSay.Id > 0)
            {
                yourSayData.Id = yourSay.Id;
                return true;
            }
            return false;
        }

        public async Task<bool> SaveYourSayAttachment(YourSay yourSay)
        {
            if (!yourSay.HasAttachment || string.IsNullOrEmpty(yourSay.PhotoName)) return true;
            bool attachmentUploaded = false;
            int retryCount = 0;
            while (retryCount < 3 && !attachmentUploaded)
            {
                try
                {
                    attachmentUploaded = await _clientManager.UploadAttachment(_listName, yourSay.Id, yourSay.Photo, yourSay.PhotoName);
                }
                catch (Exception ex)
                {
                    _analyticsService.ReportException(ex, new Dictionary<string, string> {
                        { "Message", $"Failed to upload Your Say attachment, retry count: {retryCount}" }
                    });
                    retryCount++;
                }
            }
            return attachmentUploaded;
        }

        public List<string> GetTravelDirections()
        {
            return new List<string> { "To City", "From City", "North", "South", "East", "West", "Down", "Up" };
        }

        public List<Route> LoadJourneyRoutes()
        {
            var routes = Barrel.Current.Get<List<Route>>(Consts.RouteDataKey);
            if (routes == null || routes.Count == 0)
            {
                lock (_lock)
                {
                    routes = Barrel.Current.Get<List<Route>>(Consts.RouteDataKey);
                    if (routes == null || routes.Count == 0)
                    {
                        routes = _clientManager.GetAllItems<Route>("Routes").Result;
                        Barrel.Current.Add(key: Consts.RouteDataKey, data: routes, expireIn: TimeSpan.FromDays(1));
                    }
                }
            }
            return routes ?? new List<Route>();
        }

        private async Task<YourSay> CreateYourSay(YourSay yourSay)
        {
            var yourSayDTO = new YourSayRestDTO(yourSay);
            var result = await _clientManager.CreateItem<YourSay>("Your Say", yourSayDTO);
            return result;
        }
    }

    public class YourSayRestDTO
    {
        public YourSayRestDTO(YourSay yourSay)
        {
            FormType = yourSay.FormType;
            ExperienceDate = yourSay.ExperienceDate;
            Title = yourSay.Title;
            FeedbackMessage = yourSay.FeedbackMessage;
            TravelRelated = yourSay.TravelRelated;
            ContactUser = yourSay.ContactUser;
            Route = yourSay.Route;
            JourneyFrom = yourSay.JourneyFrom;
            JourneyTo = yourSay.JourneyTo;
            TravelDirection = yourSay.TravelDirection;
            TramNumber = yourSay.TramNumber;
            Email = yourSay.Email;
            MobileNumber = yourSay.MobileNumber;
            //PhotoName = PhotoName;
            __metadata = new Metadata { type = "SP.Data.YourSayListItem" };
        }

        public YourSayRestDTO() { }

        public YourSay GetYourSay()
        {
            return new YourSay()
            {
                //Id = Id,
                Title = Title,
                // PhotoName = PhotoName
            };
        }
        public Metadata __metadata { get; set; }
        public string Title { get; set; }
        public string FormType { get; set; } // Complaint/Feedback/Suggestion/Safety
        public DateTime ExperienceDate { get; set; }
        public string FeedbackMessage { get; set; }
        public bool TravelRelated { get; set; }
        public bool ContactUser { get; set; }
        //public string PhotoName { get; set; }
        //public byte[] Photo { get; set; }
        public string Route { get; set; }
        public string JourneyFrom { get; set; }
        public string JourneyTo { get; set; }
        public string TravelDirection { get; set; }
        public string TramNumber { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public int Id { get; set; }
    }



}
