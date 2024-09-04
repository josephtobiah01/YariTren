using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using Core.Models;

namespace SharedLogic
{
    public sealed class SPOnlineAuthenticator
    {
        /// <summary>
        /// Authenticate with O365. The returned bearer token can be added to the request authorization header.
        /// </summary>
        /// <param name="siteUrl"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public UserCredential Authenticate(string siteUrl, string username, SecureString password)
        {

            return new UserCredential()
            {
                Username = username,
                Password = password,
            };
            //TODO: this method not implemented
        }
    }
}
