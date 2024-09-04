using Core;
using Core.Analytics;
using Core.Models;
using Data.SharePoint.Authentication;
using Data.SharePoint.Clients;
using MonkeyCache.FileStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharedLogic
{
    public class YourSayManager
    {
        private static object _lock = new object();

        public static async Task<bool> SaveYourSay(YourSay yourSayData)
        {
            var authenticator = FreshIOC.Container.Resolve<SPAuthenticator>();
            var client = new YourSayClient(Consts.SiteUrl, authenticator);
            int retryCount = 0;
            YourSay yourSay = null;
            await Task.Factory.StartNew(async () =>
            {
                while (retryCount < 3)
                {
                    try
                    {
                        yourSay = await client.CreateListItem(yourSayData);
                        break;
                    }
                    catch (Exception ex)
                    {
                        string message = $"Failed to save Your Say list item, retry count: {retryCount}";
                        AnalyticsService.ReportException(ex, message);
                    }
                    finally
                    {
                        retryCount++;
                    }
                }
            });
            if (yourSay != null && yourSay.Id > 0)
            {
                yourSayData.Id = yourSay.Id;
                return true;
            } 
            return false;
        }

        public static async Task<bool> SaveYourSayAttachment(YourSay yourSay)
        {
            bool attachmentUploaded = false;
            var authenticator = FreshIOC.Container.Resolve<SPAuthenticator>();
            var client = new YourSayClient(Consts.SiteUrl, authenticator);
            if (!yourSay.HasAttachment) return true;
            int retryCount = 0;
            await Task.Factory.StartNew(async () =>
            {
                while (retryCount < 3)
                {
                    try
                    {
                        // This might take a while - run it on a background thread and let the UI continue
                        attachmentUploaded = await client.UploadAttachment(yourSay.Id, yourSay.Photo, yourSay.PhotoName);
                        break;
                    }
                    catch (Exception ex)
                    {
                        string message = $"Failed to save Your Say list item, retry count: {retryCount}";
                        AnalyticsService.ReportException(ex, message);
                    }
                    finally
                    {
                        retryCount++;
                    }
                }
            });
            return attachmentUploaded;
        }

        public static List<string> GetTravelDirections()
        {
            var Items = new List<string>()
            {
                "To City",
                "From City",
                "North",
                "South",
                "East",
                "West",
                "Down",
                "Up"
            };        
            return Items;
         }

        public static List<Route> LoadJourneyRoutes()
        {
            var routes = Barrel.Current.Get<List<Route>>(Consts.RouteDataKey);
            if (routes == null)
            {
                lock (_lock)
                {
                    routes = Barrel.Current.Get<List<Route>>(Consts.RouteDataKey);
                    if (routes == null)
                    {
                        var authenticator = FreshIOC.Container.Resolve<SPAuthenticator>();
                        var client = new RoutesClient(Consts.SiteUrl, authenticator);
                        routes = client.GetAll().ToList();
                        //Cache User profile data in app to use later - cache for one day. User Profile Properties can change
                        Barrel.Current.Add(key: Consts.RouteDataKey, data: routes, expireIn: TimeSpan.FromDays(1));
                        return routes;
                    }
                }
            }
            if (routes == null) return new List<Route>();
            return routes;
        }
    }
}
