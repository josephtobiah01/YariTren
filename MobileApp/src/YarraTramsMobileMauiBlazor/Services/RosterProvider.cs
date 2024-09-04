//using Core;
//using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTramsMobileMauiBlazor.Interfaces;

namespace YarraTramsMobileMauiBlazor.Services
{
    public class RosterProvider : IRosterProvider
    {
        private ApiManager _apiManager;
        private List<RosterRecord> _data;
        public event EventHandler<List<RosterRecord>> RosterLoaded;
        public event EventHandler<string> RosterLoadFailed;
        public bool InProgress { get; set; }

        public RosterProvider(ApiManager apiManager)
        {
            _apiManager = apiManager;
        }

        public bool HasRoster(UserProfile userProfile)
        {
            return userProfile != null && userProfile.EmployeeRole == Consts.UserRoleTramDriver && !string.IsNullOrEmpty(userProfile.EmployeeNumber);
        }

        public async Task LoadRosterAsync(UserProfile userProfile, bool reload = false)
        {
            if (InProgress && !reload) return;
            if (_data != null && !reload)
            {
                RosterLoaded?.Invoke(this, _data);
                return;
            }

            InProgress = true;
            try
            {
                var rosterRecords = await FetchRosterDataAsync(userProfile);
                if (rosterRecords != null && rosterRecords.Any())
                {
                    _data = rosterRecords.ToList();
                    RosterLoaded?.Invoke(this, _data);
                }
                else
                {
                    RosterLoadFailed?.Invoke(this, "No roster data found.");
                }
            }
            catch (Exception ex)
            {
                RosterLoadFailed?.Invoke(this, ex.Message);
            }
            finally
            {
                InProgress = false;
            }
        }

        private async Task<IEnumerable<RosterRecord>> FetchRosterDataAsync(UserProfile userProfile)
        {
            if (!HasRoster(userProfile)) return Enumerable.Empty<RosterRecord>();

            string endpoint = $"{Consts.SiteUrl}/{Consts.RosterSubsite}/_api/web/lists/getbytitle('{Consts.RosterListName}')/Items";
            var rosterRecords = await _apiManager.GetForResponseAsync<IEnumerable<RosterRecord>>(endpoint);
            var confirmedRosterRecords = await _apiManager.GetForResponseAsync<IEnumerable<RosterConfirmation>>(Consts.RosterConfirmationListName);

            var confirmedIndices = confirmedRosterRecords.Where(c => c.RosterConfirmed).Select(c => c.DepotWeekIndex).ToHashSet();
            var confirmedRecords = rosterRecords.Where(r => confirmedIndices.Contains(r.DepotWeekIndex)).ToList();

            return confirmedRecords;
        }
    }

}
