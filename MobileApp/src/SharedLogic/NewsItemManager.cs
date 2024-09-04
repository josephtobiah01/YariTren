using Core;
using Core.Models;
using Data.SharePoint.Authentication;
using System.Collections.Generic;

namespace SharedLogic
{
    public class NewsItemManager
    {
        public int GetNewNewsItemForCurrentUserCount(UserProfile userProfile)
        {
            var authenticator = FreshIOC.Container.Resolve<SPAuthenticator>();
            var client = new NewsClient(Consts.NewsSiteUrl, authenticator);
            var newNewsItems = client.GetAllNewUntil();

            var alertsClient = new AlertsClient(Consts.SiteUrl, authenticator);
            var newAlertItems = alertsClient.GetAllNewUntil();

            List<NewsItem> newUntilItems = new List<NewsItem>();
            if (newNewsItems != null)
            {
                newUntilItems.AddRange(newNewsItems);
            }
            if (newAlertItems != null)
            {
                newUntilItems.AddRange(newAlertItems);
            }
            return ProcessNewsItems(userProfile, newUntilItems);
        }

        internal static int ProcessNewsItems(UserProfile userProfile, IList<NewsItem> newItems)
        {
            if (newItems == null || newItems.Count == 0) return 0;

            int badgeCount = 0;
            foreach (var newItem in newItems)
            {
                if (newItem.UserAudience != null && newItem.UserAudience.Count > 0 && !newItem.UserAudience.Contains(userProfile.EmployeeRole)) continue;
                if (newItem.LocationAudience != null && newItem.LocationAudience.Count > 0 && !newItem.LocationAudience.Contains(userProfile.Office)) continue;
                if (newItem.ManagementLevel != null && newItem.ManagementLevel.Count > 0 && !newItem.ManagementLevel.Contains(userProfile.ManagementLevel)) continue;
                if (newItem.Function != null && newItem.Function.Count > 0 && !newItem.Function.Contains(userProfile.Function)) continue;
                badgeCount++;
            }
            return badgeCount;
        }

    }
}
