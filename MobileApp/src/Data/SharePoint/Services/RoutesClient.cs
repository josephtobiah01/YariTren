using Core.Models;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;

namespace Data.SharePoint.Clients
{
    public class RoutesClient : ClientBase<Route>
    {
        public RoutesClient(string siteUrl, IAuthenticator authenticator) : base(siteUrl, authenticator)
        {
            ListName = "Routes";
            string selectFields = "Id,Title,Code";
            ODataQuery = string.Format("?$select={0}", selectFields);
        }
    }
}
