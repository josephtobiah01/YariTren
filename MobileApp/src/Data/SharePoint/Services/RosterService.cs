using Core;
using Core.Helpers;
using Core.Interfaces;
using Core.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.ApplicationModel.Communication;
using MonkeyCache.FileStore;
using RestSharp;
using RestSharp.Authenticators;
using SharedLogic.Roster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Data.SharePoint.Services
{
    /// <summary>
    /// Class used to return roster data that is associated for the current user
    /// </summary>
    /// <seealso cref="Data.SharePoint.Clients.ClientBase{Core.Models.RosterConfirmation}" />
    public class RosterService : IRosterService /*ClientBase<RosterConfirmation>*/
    {
        private readonly IApiManager _apiManager;
        private readonly IClientManager _clientManager;
        private readonly ILogger<RosterService> _logger;
        private List<RosterRecord> _rosterData;
        private bool _inProgress;

        //public RestClient restClient;
        public event EventHandler<string> RosterLoadFailed;
        public event EventHandler<RosterLoadedEventArgs>? RosterLoaded;
        public RosterService(
            IApiManager apiManager,
            ILogger<RosterService> logger,
            IClientManager clientManager)
        {
            
            _apiManager = apiManager;
            _logger = logger;
            _clientManager = clientManager;
            Barrel.ApplicationId = "com.companyname.YarraTramsMobileMauiBlazor"; // Setup caching
        }

        public bool HasRoster(UserProfile userProfile)
        {
            //return userProfile != null && userProfile.EmployeeRole == Consts.UserRoleTramDriver && !string.IsNullOrEmpty(userProfile.EmployeeNumber);
            return userProfile != null && !string.IsNullOrEmpty(userProfile.EmployeeNumber);
        }

        public async Task<List<RosterRecord>> Load(UserProfile userProfile, bool reload = false)
        {
            if (!HasRoster(userProfile)) return new List<RosterRecord>();

            if (_inProgress) return _rosterData;
            _inProgress = true;

            try
            {
                if (reload)
                {
                    var rosterRecords = await GetConfirmedRosterData(userProfile);
                    if (rosterRecords != null)
                    {
                        _rosterData = rosterRecords.ToList();
                        OnRosterLoaded(rosterRecords);
                    }
                    else
                    {
                        OnRosterLoadFailed(new Exception("No roster data found"), "No roster data found.");
                    }
                    Barrel.Current.Add(Consts.RosterDataKey, rosterRecords, TimeSpan.FromDays(365));

                    return rosterRecords;
                }
                return new List<RosterRecord>();
                
            }
            catch (Exception ex)
            {
                OnRosterLoadFailed(ex, "Failed to load or process roster data");
                return new List<RosterRecord>();
            }
            finally
            {
                _inProgress = false;
            }
        }

        public async Task<List<RosterRecord>> GetConfirmedRosterData(UserProfile userProfile)
        {
            var rosterConfirmations = await GetRosterConfirmationsAsync();
            var rosterEntries = await GetRosterEntriesAsync(userProfile.EmployeeNumber);

            return rosterEntries.Where(entry =>
                rosterConfirmations.Any(confirm =>
                    confirm.DepotWeekIndex == entry.DepotWeekIndex && confirm.RosterConfirmed))
                .ToList();
        }

        private async Task<List<RosterConfirmation>> GetRosterConfirmationsAsync()
        {
            string requestUrl = $"{GetRosterSubSite()}/_api/web/lists/GetByTitle('{Consts.RosterConfirmationListName}')/items?$select=Id,WorkdayDate,Depot,DepotWeekIndex,RosterConfirmed&$filter=WorkdayDate ge datetime'{DateTime.Today.AddDays(-35).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")}'&$orderby=WorkdayDate";

            try
            {
                var items = await _clientManager.GetItemsByCustomUrl<RosterConfirmation>(requestUrl);
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Australia/Melbourne");
                if (items != null)
                {
                    //Note: SharePoint returns us UTC times for the WorkdayDate, so we need to convert these to local time.  
                    foreach (var item in items)
                    {
                        item.WorkdayDate = TimeZoneInfo.ConvertTimeFromUtc(item.WorkdayDate, timeZone);
                    }
                }
                return items.ToList() ?? new List<RosterConfirmation>();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to fetch roster confirmations: {0}", ex.Message);
                throw new Exception("API call to fetch roster confirmations failed", ex);
            }
        }

        private async Task<List<RosterRecord>> GetRosterEntriesAsync(string employeeNumber)
        {
            string requestUrl = $"{GetRosterSubSite()}/_api/web/lists/GetByTitle('{Consts.RosterListName}')/items?$select=Id,WorkdayDate,EmployeeID,EmployeeName,LocationStart,Depot,DutyID,DutyType,DepotWeekIndex,StartTime,MealLocation,MealStart,MealEnd,EndTime,WorkingDuration,SplitDuration,Employee/ID,Employee/Name,Employee/SipAddress&$expand=Employee&$filter=EmployeeID eq '{employeeNumber}' and WorkdayDate ge datetime'{DateTime.Today.AddDays(-28).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")}'&$orderby=WorkdayDate";
            try
            {
                var items = await _clientManager.GetItemsByCustomUrl<RosterRecord>(requestUrl);
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById("Australia/Melbourne");
                if (items != null)
                {
                    //Note: SharePoint returns us UTC times for the WorkdayDate, so we need to convert these to local time.  
                    foreach (var item in items)
                    {
                        item.WorkdayDate = TimeZoneInfo.ConvertTimeFromUtc(item.WorkdayDate, timeZone);
                    }
                }
                return items.ToList() ?? new List<RosterRecord>();
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to fetch roster entries: {0}", ex.Message);
                throw new Exception("API call to fetch roster entries failed", ex);
            }
        }

        public void OnRosterLoaded(List<RosterRecord> data)
        {
            RosterLoaded?.Invoke(this, new RosterLoadedEventArgs(data));
        }

        private void OnRosterLoadFailed(Exception ex, string reason)
        {
            RosterLoadFailed?.Invoke(this, reason);
        }

        internal static string GetRosterSubSite()
        {
            return HttpUtility.ConcatUrls(Consts.SiteUrl, Consts.RosterSubsite);
        }

    }
}
