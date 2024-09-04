using Core;
using Core.Interfaces;
using Core.Models;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace Data.SharePoint.Clients
{
    /// <summary>
    /// Class used to return roster data that is associated for the current user
    /// </summary>
    /// <seealso cref="Data.SharePoint.Clients.ClientBase{Core.Models.RosterRecord}" />
    public class RosterClient : ClientBase<RosterRecord>
    {
        private readonly IAnalyticsService _analyticsService;
        public RosterClient(
            string siteUrl, 
            IAuthenticator authenticator, 
            string employeeNumber,
            IAnalyticsService analyticsService) : base(siteUrl, authenticator, analyticsService)
        {
            ListName = Consts.RosterListName;

            // The folllowing query is used:
            // Top = 200 - return maximum of 200 records. The current requirement is to show prev 4 and upcoming 4 weeks/, ie 52 days worth - so a limit of 200 is just an extra pre-caution to not return too much data, as the roster list will contains 1000's of items
            // Select - Everything from the roster list item
            // Filter - Only return those records where the EmployeeNumber matches the given employeenumber & where records are from 4 weeks ago onwards
            // Sort - Order by WorddayDate asc

            string selectFields = "Id,WorkdayDate,EmployeeID,EmployeeName,LocationStart,Depot,DutyID,DutyType,StartTime,MealLocation,MealStart,MealEnd,EndTime,WorkingDuration,SplitDuration,Employee/ID,Employee/Name,Employee/SipAddress";
            ODataQuery = $"?$top=100&$select={selectFields}&$expand=Employee&$filter=EmployeeID%20eq%20%27{employeeNumber}%27%20and%20WorkdayDate%20ge%20datetime'{DateTime.Today.AddDays(-28).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")}'&$sort=WorkdayDate";
        }
    }
}
