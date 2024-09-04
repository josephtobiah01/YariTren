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
<<<<<<< HEAD
            //TODO: lets turn this into an async method
            string key = DatabaseManager.GetKey().Result;
            var credentials = CredentialManager.GetCredentials(key).Result;           
            var client = new RosterClient(GetRosterSubSite(), credentials.Username, credentials.Password);
            return client.GetAll().ToList();
=======
            List<RosterRecord> items = new List<RosterRecord>();
            for (var i = 0; i < 14; i++)
            {
                items.Add(new RosterRecord()
                {
                    WorkdayDate = new DateTime(2018, 11,15 + i, 0, 0, 0),
                    EmployeeID = "12345",
                    EmployeeName = "Test Employee",
                    LocationStart = "sb06",
                    DutyID = "A/L",
                    DutyType = "E",
                    StartTime = "3:15",
                    EndTime = "12:31",
                    MealStart = "7:31",
                    MealEnd = "8:22",
                    MealLocation = "sb06",
                    Depot = "SBANK",
                    WorkingDuration = "0h00",
                    SplitDuration = "0h00"
                });
            }
            items[8].IsCDO = true;
            return items;
>>>>>>> Basic foundation of Top and bottom bar
        }

        public static string GetRosterSubSite()
        {
            return HttpUtility.ConcatUrls(Consts.SiteUrl, Consts.RosterSubsite);
        }
    }
}
