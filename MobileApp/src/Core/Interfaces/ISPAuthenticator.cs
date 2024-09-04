using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISPAuthenticator : IAuthenticator
    {
        string AccessToken { get; }
        bool Authenticated { get; }
        bool CredentialsOK { get; }
        Task<bool> IsAuthenticated();
        Task PreAuthenticate(bool credentialCheck = false);
        void ClearTokenCache();
    }
}
