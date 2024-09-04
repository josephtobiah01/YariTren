using Core.Interfaces;
using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTramsMobileMauiBlazor.Interfaces;

namespace YarraTramsMobileMauiBlazor.Services
{
    public class UtilitiesService : IUtilitiesService
    {
        IAnalyticsService? _analyticsService => ServiceLocator.Current?.GetService<IAnalyticsService>();
        IUserProfileService? _userProfileService => ServiceLocator.Current?.GetService<IUserProfileService>();
        public void TrackCustomUserAction(string area, string message)
        {
            var properties = new Dictionary<string, string> { { "Area", area }, { "Message", message } };
            _analyticsService?.ReportEvent(message, properties);
        }

        public async void TrackUserAction(string data)
        {
            string message = string.Format("{0} - opened", data);
            string role = "Not set";
            string location = "Not set";
            var user = await GetCurrentUserProfile();
            if (user != null)
            {
                if (!string.IsNullOrEmpty(user.EmployeeRole)) role = user.EmployeeRole;
                if (!string.IsNullOrEmpty(user.Office)) location = user.Office;
            }
            var properties = new Dictionary<string, string> { { "Area", data }, { "Message", message }, { "Role", role }, { "Location", location } };
            _analyticsService?.ReportEvent(message, properties);
        }

        public async Task<UserProfile> GetCurrentUserProfile()
        {
            UserProfile userProfile = new UserProfile();
            userProfile = await _userProfileService.GetCurrentUserProfile();
            return userProfile;
        }
    }
}
