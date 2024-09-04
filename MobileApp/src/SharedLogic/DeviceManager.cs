using Core;
using Core.Helpers;
using Core.Models;
using Data.SharePoint.Authentication;
using System.Collections.Generic;

namespace SharedLogic
{
    public class DeviceManager
    {
        // Method to sync device details to Device Info List (should include EmployeeRole, Office/Depot)
        public static void SetUserDeviceInfo(UserDevice userDevice, UserProfile userProfile)
        {
            // Set the user profile values on the device object (as we'll save these out all together)
            userDevice.EmployeeName = userProfile.DisplayName;
            userDevice.EmployeeID = userProfile.EmployeeNumber ?? null;
            userDevice.Office = userProfile.Office ?? null;
            if (userDevice.LocationAudience == null) { userDevice.LocationAudience = new List<string>(); }
            if (userDevice.UserAudience == null) { userDevice.UserAudience = new List<string>(); }
            if (userDevice.ManagementLevel == null) { userDevice.ManagementLevel = new List<string>(); }
            if (userDevice.Function == null) { userDevice.Function = new List<string>(); }

            userDevice.LocationAudience.Add(userProfile.Office);
            userDevice.UserAudience.Add(userProfile.EmployeeRole);
            userDevice.ManagementLevel.Add(userProfile.ManagementLevel);
            userDevice.Function.Add(userProfile.Function);
            SetTags(userDevice);
            //TODO: lets turn this into an async method
            var authenticator = FreshIOC.Container.Resolve<SPAuthenticator>();
            UserDeviceService udc = new UserDeviceClient(Consts.SiteUrl, authenticator);
            var retVal = udc.CreateOrUpdateListItem(userDevice);
        }

        internal static void SetTags(UserDevice userDevice)
        {
            string tags = string.Empty;
            if (userDevice.LocationAudience != null && userDevice.LocationAudience.Count > 0)
            {
                foreach (var val in userDevice.LocationAudience)
                {
                    if (string.IsNullOrEmpty(val)) continue;
                    if (string.IsNullOrEmpty(tags))
                    {
                        tags = string.Format("{0};", val);
                        continue;
                    }
                    tags = string.Format("{0}{1};", tags, val);
                }
            }
            if (userDevice.UserAudience != null && userDevice.UserAudience.Count > 0)
            {
                foreach (var val in userDevice.UserAudience)
                {
                    if (string.IsNullOrEmpty(val)) continue;
                    if (string.IsNullOrEmpty(tags))
                    {
                        tags = string.Format("{0};", val);
                        continue;
                    }
                    tags = string.Format("{0}{1};", tags, val);
                }
            }

            if (userDevice.ManagementLevel != null && userDevice.ManagementLevel.Count > 0)
            {
                foreach (var val in userDevice.ManagementLevel)
                {
                    if (string.IsNullOrEmpty(val)) continue;
                    if (string.IsNullOrEmpty(tags))
                    {
                        tags = string.Format("{0};", val);
                        continue;
                    }
                    tags = string.Format("{0}{1};", tags, val);
                }
            }

            if (userDevice.Function != null && userDevice.Function.Count > 0)
            {
                foreach (var val in userDevice.Function)
                {
                    if (string.IsNullOrEmpty(val)) continue;
                    if (string.IsNullOrEmpty(tags))
                    {
                        tags = string.Format("{0};", val);
                        continue;
                    }
                    tags = string.Format("{0}{1};", tags, val);
                }
            }

            if (!string.IsNullOrEmpty(tags))
            {
                userDevice.Tags = Utility.ConvertStringToTag(tags); // remove spaces and ampersands
            }
        }

    }
}
