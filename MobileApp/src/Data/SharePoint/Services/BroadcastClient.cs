using Core.Interfaces;
using Core.Models;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security;
using System.Text;

namespace Data.SharePoint.Clients
{
    public class BroadcastClient 
    {
        public BroadcastClient(
            string siteUrl, 
            IAuthenticator authenticator) : base(siteUrl, authenticator)
        {
            ListName = "BroadCasts";
        }
    }
}
