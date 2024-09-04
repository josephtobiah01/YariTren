using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using Core.Helpers;
using Core.Models;
using Data.SharePoint.Clients;

namespace SharedLogic
{
    public static class RosterManager
    {
        /// <summary>
        /// Gets the roster data for the current user
        /// </summary>
        /// <returns></returns>
        public static List<RosterRecord> GetRoster()
        {
            //TODO: lets turn this into an async method
            string key = DatabaseManager.GetKey().Result;
            var credentials = CredentialManager.GetCredentials(key).Result;           
            var client = new RosterClient(GetRosterSubSite(), credentials.Username, credentials.Password);
            return client.GetAll().ToList();
        }

        public static string GetRosterSubSite()
        {
            return HttpUtility.ConcatUrls(Consts.SiteUrl, Consts.RosterSubsite);
        }
    }
}
