using Core;
using Core.Helpers;
using Core.Interfaces;
using Core.Models;
using Data.SharePoint.Authentication;
using MonkeyCache.FileStore;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Data.SharePoint.Clients
{
    /// <summary>
    /// Used to retrieve the SharePoint profile for the current user
    /// </summary>
    /// <seealso cref="Data.SharePoint.Clients.ClientBase{Core.Models.UserProfile}" />
    public class UserProfileService : IUserProfileService
    {

        private readonly IClientManager _clientManager;
        private readonly string _siteUrl = Consts.SiteUrl;
        private readonly IHttpUtility _httpUtility;
        private readonly IRosterService _rosterService;
        private readonly IDatabaseService _databaseService;
        private readonly ICookieService _cookieService;
        private const int _daysToCacheProfile = 1;

        private const string CurrentUserProfileAPI = "_api/SP.UserProfiles.PeopleManager/GetMyProperties";
        private const string SpecificUserProfileAPI = "/_api/SP.UserProfiles.PeopleManager/GetPropertiesFor(accountName=@v)?@v='{0}'";


        public UserProfileService(
            IClientManager clientManager,
            IHttpUtility httpUtility,
            
            IDatabaseService databaseService,
            ICookieService cookieService,
            IRosterService rosterService)
        {
            _clientManager = clientManager;
            _httpUtility = httpUtility;
            _databaseService = databaseService;
            _cookieService = cookieService;
            _rosterService = rosterService;
        }

        public async Task<UserProfile> GetCurrentUserProfile()
        {
            var userProfile = Barrel.Current.Get<UserProfile>(Consts.UserProfileKey);

            if (userProfile == null)
            {
                string apiUrl = _httpUtility.ConcatUrls(_siteUrl, CurrentUserProfileAPI);
                userProfile = await GetSingleUserProfile(apiUrl);

                AddUserProfileToCache(userProfile, _daysToCacheProfile);

                if (_rosterService.HasRoster(userProfile))
                {
                    await _rosterService.Load(userProfile, true);
                }
            }
            return userProfile;
        }

        public async Task<UserProfile> GetUserProfile(string loginName)
        {
            string encodedLoginName = System.Net.WebUtility.UrlEncode(loginName);
            string apiUrl = _httpUtility.ConcatUrls(_siteUrl, string.Format(SpecificUserProfileAPI, encodedLoginName));
            return await GetSingleUserProfile(apiUrl);
        }

        private async Task<UserProfile> GetSingleUserProfile(string apiUrl)
        {
            var response = await _clientManager.GetItemByCustomUrl<UserProfile>(apiUrl);
            return response;
        }

        public bool HasUserProfile()
        {
            try
            {
                bool hasProfile = false;
                AsyncHelper.RunSync(async () =>
                {
                    var profile = await _databaseService.GetUser();
                    hasProfile = profile != null && !string.IsNullOrEmpty(profile.EncryptedPinHash);
                });
                return hasProfile;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void AddUserProfileToCache(UserProfile userProfile, int daysToCache = _daysToCacheProfile)
        {
            if (userProfile == null) return;
            if (Barrel.Current.Exists(Consts.UserProfileKey))
            {
                Barrel.Current.Empty(Consts.UserProfileKey);
            }
            Barrel.Current.Add(key: Consts.UserProfileKey, data: userProfile, expireIn: TimeSpan.FromDays(daysToCache));
        }

        public void ResetUserDetails()
        {
            _databaseService.ClearTables();
            Barrel.Current.EmptyAll();
            SPAuthenticator.Instance.ClearTokenCache();
            _cookieService.ClearAllCookies();
        }
    }
}
