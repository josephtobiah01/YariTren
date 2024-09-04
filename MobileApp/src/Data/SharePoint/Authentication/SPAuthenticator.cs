using Core.Exceptions;
using Core.Helpers;
using Core.Models;
using Core.Security;
using Core.Analytics;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Diagnostics;
using Core;
using Microsoft.Identity.Client;
using System.Runtime.CompilerServices;
using System.Threading;
using Core.Interfaces;
using System.ComponentModel;
using static System.Formats.Asn1.AsnWriter;

namespace Data.SharePoint.Authentication
{
    /// <summary>
    /// The code below uses MSAL library to retreive an Access Token to be used with API calls.
    /// </summary>
    public class SPAuthenticator
    {
        private IPublicClientApplication publicClientApplication;
        private string _accessToken;
        private const string azureAdEndPoint = "https://login.microsoftonline.com";
        private IAnalyticsService _analyticsService;
        private string _siteUrl = Consts.SiteUrl;

        public static SPAuthenticator Instance { get; private set; } = new SPAuthenticator();

        // Android uses this to determine which activity to use to show
        // the login screen dialog from.
        public static object ParentWindow { get; set; }

        internal IPublicClientApplication PCA { get; }

        public string AccessToken
        {
            get => _accessToken;
            private set => _accessToken = value;
        }

        // private constructor for singleton
        public SPAuthenticator()
        {
#if ANDROID
            PCA = PublicClientApplicationBuilder
                .Create(Consts.AzureADClientId)
                .WithAuthority(string.Concat(azureAdEndPoint, "/organizations/"), true)
                .WithRedirectUri(PlatformConfig.Instance.RedirectUri)
                .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                .WithBroker(true)
                .Build();
#else
            PCA = PublicClientApplicationBuilder
                .Create(Consts.AzureADClientId)
                .WithAuthority(string.Concat(azureAdEndPoint, "/organizations/"), true)
                .WithRedirectUri(PlatformConfig.Instance.RedirectUri)
                .WithIosKeychainSecurityGroup("com.microsoft.adalcache")
                .Build();
#endif
        }

        public string[] GetDefaultScope()
        {
            //string baseScope = "Tenant.default";
            //return new[] { baseScope };

            string baseScope = HttpUtility.ConcatUrls(Tenant, ".default");
            string[] scopes = { baseScope };
            return scopes;
        }

        protected string Tenant
        {
            get
            {
                return HttpUtility.GetHostFromFullUrl(_siteUrl);
            }
        }

        ///// <summary>
        ///// Authenticates the specified client. This is called on each client request.
        ///// When this is first called it will authenticate using the supplied user name and password, and retrieve cookie and digest for subsequent calls.
        ///// On subsequent calls the cookie and digest value from the inital authentication pass are used.
        ///// </summary>
        ///// <param name="client">The client.</param>
        ///// <param name="request">The request.</param>
        //public ValueTask Authenticate(IRestClient client, RestRequest request)
        //{
        //    return request.AddHeader("Authorization", $"Bearer {AccessToken}");
        //}

        // This method is purely to check if we already have an Access Token
        public async Task<bool> IsAuthenticated()
        {
            AccessToken = await GetAccessToken(false, true); // get access token silently (ie cached)
            return !string.IsNullOrEmpty(AccessToken);

            //AuthenticationResult authResult = null;
            //string[] scopes = GetDefaultScope();
            //try
            //{
            //    authResult = await GetCachedAccessTokenAsync(scopes, CancellationToken.None);
            //}
            //catch (MsalUiRequiredException)
            //{
            //    // Log the exception for debugging purposes
            //    Console.WriteLine("MsalUiRequiredException caught, attempting to acquire token interactively.");
            //}

            //// Check if the access token is still valid
            //if (authResult != null && authResult.ExpiresOn > DateTimeOffset.UtcNow)
            //{
            //    return true;
            //}

            //// If silent acquisition failed or token expired, acquire a new token interactively
            //authResult = await GetNewAccessTokenAsync(scopes, CancellationToken.None);
            //if (authResult != null && authResult.ClaimsPrincipal.Identity.IsAuthenticated)
            //{
            //    // Update the access token
            //    AccessToken = authResult.AccessToken;
            //    return true;
            //}
            //return false;
        }

        public async Task<bool> Login()
        {
            AccessToken = await GetAccessToken(); // get access token silently, but if this fails, put the user through the MSAL login process to retrieve new token
            return !string.IsNullOrEmpty(AccessToken);
        }

        //public async Task PreAuthenticate(bool credentialCheck = false)
        //{
        //    int retryAttempt = 0;
        //    bool retry = true && !credentialCheck;

        //    while (retry)
        //    {
        //        try
        //        {
        //            AccessToken = await GetAccessToken(credentialCheck);
        //            if (AccessToken == null) throw new TokenRetrievalException("Failed to successfully retrieve access token");
        //            if (credentialCheck) return;
        //            retry = false;
        //        }
        //        catch (Exception ex)
        //        {
        //            if (ex is WebException webEx && webEx.Status == WebExceptionStatus.ProtocolError)
        //            {
        //                retry = false;
        //            }
        //            else if (retryAttempt < 3)
        //            {
        //                retryAttempt++;
        //                await Task.Delay(1000); // wait 1 second before retrying
        //            }
        //            else
        //            {
        //                retry = false;
        //            }
        //        }
        //    }
        //}

        public async Task<string> GetAccessToken(bool credsCheck = false, bool silent = false)
        {
            string[] scopes = GetDefaultScope();
            return await GetAccessTokenAsync(scopes, credsCheck, silent);
        }

        public async Task<string> GetAccessTokenAsync(string[] scopes, bool credsCheck = false, bool silent = false)
        {
            return await GetAccessTokenAsync(scopes, CancellationToken.None, credsCheck, silent);
        }

        internal async Task<string> GetAccessTokenAsync(string[] scopes, CancellationToken cancellationToken, bool credsCheck = false, bool silent = false)
        {
            AuthenticationResult authResult = null;

            if (credsCheck)
            {
                authResult = await GetNewAccessTokenAsync(scopes, cancellationToken);
            }
            else
            {
                try
                {
                    authResult = await GetCachedAccessTokenAsync(scopes, cancellationToken);
                }
                catch (MsalUiRequiredException)
                {
                    if (silent)
                    {
                        return null;
                    }
                    authResult = await GetNewAccessTokenAsync(scopes, cancellationToken);
                }
            }
            return authResult?.AccessToken;
        }
        //internal async Task<string> GetTenantIdAsync(string[] scopes, bool credsCheck = false)
        //{
        //    return await GetTenantIdAsync(scopes, CancellationToken.None, credsCheck);
        //}
        //internal async Task<string> GetTenantIdAsync(string[] scopes, CancellationToken cancellationToken, bool credsCheck = false)
        //{
        //    AuthenticationResult authResult = null;

        //    if (credsCheck)
        //    {
        //        authResult = await GetNewAccessTokenAsync(scopes, cancellationToken);
        //    }
        //    else
        //    {
        //        try
        //        {
        //            authResult = await GetCachedAccessTokenAsync(scopes, cancellationToken);
        //        }
        //        catch (MsalUiRequiredException)
        //        {
        //            authResult = await GetNewAccessTokenAsync(scopes, cancellationToken);
        //        }
        //    }

        //    return authResult?.TenantId;
        //}

        //internal async Task<string> GetIdTokenAsync(string[] scopes, bool credsCheck = false)
        //{
        //    return await GetIdTokenAsync(scopes, CancellationToken.None, credsCheck);
        //}
        //public async Task<string> GetIdTokenAsync(string[] scopes, CancellationToken cancellationToken, bool credsCheck = false)
        //{
        //    AuthenticationResult authResult = null;

        //    if (credsCheck)
        //    {
        //        authResult = await GetNewAccessTokenAsync(scopes, cancellationToken);
        //    }
        //    else
        //    {
        //        try
        //        {
        //            authResult = await GetCachedAccessTokenAsync(scopes, cancellationToken);
        //        }
        //        catch (MsalUiRequiredException)
        //        {
        //            authResult = await GetNewAccessTokenAsync(scopes, cancellationToken);
        //        }
        //    }

        //    return authResult?.IdToken;
        //}

        //public async Task<string> GetUniqueIdAsync(string[] scopes, bool credsCheck = false)
        //{
        //    return await GetUniqueIdAsync(scopes, CancellationToken.None, credsCheck);
        //}
        //public async Task<string> GetUniqueIdAsync(string[] scopes, CancellationToken cancellationToken, bool credsCheck = false)
        //{
        //    AuthenticationResult authResult = null;

        //    if (credsCheck)
        //    {
        //        authResult = await GetNewAccessTokenAsync(scopes, cancellationToken);
        //    }
        //    else
        //    {
        //        try
        //        {
        //            authResult = await GetCachedAccessTokenAsync(scopes, cancellationToken);
        //        }
        //        catch (MsalUiRequiredException)
        //        {
        //            authResult = await GetNewAccessTokenAsync(scopes, cancellationToken);
        //        }
        //    }

        //    return authResult?.UniqueId;
        //}

        internal async Task<AuthenticationResult> GetCachedAccessTokenAsync(string[] scopes, CancellationToken cancellationToken, bool throwOnError = true)
        {
            var accounts = await PCA.GetAccountsAsync();
            try
            {
                return await PCA.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync(cancellationToken);
            }
            catch
            {
                if (!throwOnError) return null;
                throw;
            }
        }

        internal async Task<AuthenticationResult> GetNewAccessTokenAsync(string[] scopes, CancellationToken cancellationToken)
        {
            try
            {
                return await PCA.AcquireTokenInteractive(scopes)
                    .WithParentActivityOrWindow(PlatformConfig.Instance.ParentWindow)
                    .WithUseEmbeddedWebView(true)
                    .ExecuteAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return null;
            }
        }

        public void ClearTokenCache()
        {
            Task.Run(ClearTokenCacheAsync).Wait();
        }

        public async Task ClearTokenCacheAsync()
        {
            var accounts = (await PCA.GetAccountsAsync()).ToList();
            while (accounts.Any())
            {
                await PCA.RemoveAsync(accounts.First());
                accounts = (await PCA.GetAccountsAsync()).ToList();
            }
        }

       

        #region old Xamarin app's code

        //private IPublicClientApplication publicClientApplication;
        //private readonly IAnalyticsService _analyticsService;
        //private const string azureAdEndPoint = "https://login.microsoftonline.com";
        //protected CookieContainer _cookieContainer = new CookieContainer();
        ////private const string _spAuthCookieName = "SPOIDCRL";
        ////private const string _msoStsAuthUrl = "https://login.microsoftonline.com/rst2.srf";
        ////private const string _getRealmUrl = "https://login.microsoftonline.com/GetUserRealm.srf";
        ////private const string _realm = "urn:federation:MicrosoftOnline";
        ////private const string _contextInfoPath = "/_api/contextinfo";        
        //private string _accessToken;
        //private string _username;
        ////private SecureString _password;
        //private string _siteUrl = Consts.SiteUrl;
        ////private bool _integrated = false;
        //private string _redirectUrl;
        //private UserCredential _credentials = null;
        //private int _retryCount = 3;

        //internal IPublicClientApplication PublicClientApplication
        //{
        //    get
        //    {
        //        if (publicClientApplication == null)
        //        {
        //            publicClientApplication = PublicClientApplicationBuilder
        //                .Create(Consts.AzureADClientId)
        //                .WithAuthority(string.Concat(azureAdEndPoint, "/organizations/"), true)
        //                .WithIosKeychainSecurityGroup("com.microsoft.adalcache") // required for iOS. ignored for Android
        //                .WithRedirectUri(PlatformConfig.Instance.RedirectUri)
        //                .Build();
        //        }
        //        return publicClientApplication;
        //    }
        //}

        //public bool Authenticated
        //{
        //    get { return AccessToken != null; }
        //}

        //public bool CredentialsOK
        //{
        //    get { return !string.IsNullOrEmpty(AccessToken); }
        //}

        //protected string Tenant
        //{
        //    get
        //    {
        //        return HttpUtility.GetHostFromFullUrl(_siteUrl);
        //    }
        //}

        //protected string MSODomain
        //{
        //    get
        //    {
        //        if (Tenant.EndsWith("sharepoint.com", System.StringComparison.OrdinalIgnoreCase))
        //        {
        //            return "sharepoint.com";
        //        }
        //        // else
        //        return Tenant;
        //    }
        //}

        ///// <summary>
        ///// Authenticates the specified client. This is called on each client request.
        ///// When this is first called it will authenticate using the supplied user name and password, and retrieve cookie and digest for subsequent calls.
        ///// On subsequent calls the cookie and digest value from the inital authentication pass are used.
        ///// </summary>
        ///// <param name="client">The client.</param>
        ///// <param name="request">The request.</param>
        //public void Authenticate(IRestClient client, RestRequest request)
        //{
        //    request.AddHeader("Authorization", $"Bearer {AccessToken}");
        //}

        ///// <summary>
        ///// Method that will determine if we are authenticated with the current credentials. Will retrieve and store a new Access Token if required
        ///// </summary>
        ///// <returns></returns>
        //public async Task<bool> IsAuthenticated()
        //{
        //    AccessToken = await GetAccessToken();
        //    return !string.IsNullOrEmpty(AccessToken);
        //}

        ///// <summary>
        ///// Attempts to authenticate to the configured SharePoint tenancy using the supplied credentials
        ///// If successful it retrieves a binary security token, auth cookie, and digest value that can be used for
        ///// SharePoint REST API calls
        ///// </summary>
        ///// <param name="credentialCheck">if set to <c>true</c> [credential check]. - this process involves just attempting to retrieve a binarysecuritytoken</param>
        ///// <exception cref="InvalidUsernameOrPasswordException">There was an error attempting to authenticate - please check username and password</exception>
        //public async Task PreAuthenticate(bool credentialCheck = false)
        //{
        //    int retryAttempt = 0;
        //    bool retry = true && !credentialCheck; // no retries if we are doing a credential check (credentialCheck == true)
        //    while ((_credentials == null && retry) || credentialCheck)
        //    {
        //        try
        //        {
        //            AccessToken = await GetAccessToken(credentialCheck);
        //            if (AccessToken == null) { throw new TokenRetrievalException("Failed to successfully retreive access token"); }
        //            if (credentialCheck) { return; }
        //            retry = false;
        //        }
        //        catch (WebException ex) when (ex.Status == WebExceptionStatus.ProtocolError)
        //        {
        //            string message = string.Format("Protocol error received when attempting to authenticate user: '{0}'. Login attempt: {1}", _username, retryAttempt);
        //            _analyticsService.ReportException(ex, message);
        //            retry = false;
        //        }
        //        catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.BadRequest)
        //        {
        //            //code specifically for a WebException Bad Request - 400 response
        //            string message = string.Format("Response status 400 Bad request received when attempting to authenticate user: '{0}'", _username);
        //            _analyticsService.ReportException(ex, message);
        //        }
        //        catch (InvalidUsernameOrPasswordException ex)
        //        {
        //            string message = string.Format("{0} - when attempting to authenticate user: '{1}'", ex.Message, _username);
        //            _analyticsService.ReportException(ex, message);
        //            retry = false;
        //            throw;
        //        }
        //        catch (AccountDisabledException ex)
        //        {
        //            string message = string.Format("{0} - when attempting to authenticate user: '{1}'", ex.Message, _username);
        //            _analyticsService.ReportException(ex, message);
        //            retry = false;
        //            throw;
        //        }
        //        catch (TokenRetrievalException ex)
        //        {
        //            string message = string.Format("{0} - when attempting to authenticate user: '{1}'", ex.Message, _username);
        //            _analyticsService.ReportException(ex, message);
        //            retry = false;
        //            throw;
        //        }
        //        catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.Unauthorized)
        //        {
        //            //code specifically for a WebException Unauthorized - 401 response
        //            string message = string.Format("Response status 401 Unauthorised when attempting to authenticate user: '{0}'", _username);
        //            _analyticsService.ReportException(ex, message);
        //            retry = false;
        //            throw new InvalidUsernameOrPasswordException("There was an error attempting to authenticate - please check username and password");
        //        }
        //        catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.Forbidden)
        //        {
        //            //code specifically for a WebException forbidden - 403 response
        //            string message = string.Format("Response status 403 Forbidden when attempting to authenticate user: '{0}'", _username);
        //            _analyticsService.ReportException(ex, message);
        //            retry = false;
        //        }
        //        catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.NotFound)
        //        {
        //            //code specifically for a WebException not found - 404 response
        //            string message = string.Format("Response status 404 Not Found when attempting to authenticate user: '{0}'. Login attempt: {1}", _username, retryAttempt);
        //            _analyticsService.ReportException(ex, message);
        //        }
        //        catch (WebException ex) when ((ex.Response as HttpWebResponse)?.StatusCode == HttpStatusCode.InternalServerError)
        //        {
        //            //code specifically for a WebException InternalServerError
        //            string message = string.Format("Received an internal server error response when attempting to authenticate user: '{0}'. Login attempt: {1}", _username, retryAttempt);
        //            _analyticsService.ReportException(ex, message);
        //        }
        //        //catch (MsalServiceException ex)
        //        //{
        //        //    string message = string.Format("Authentication failure for user: '{0}'", _username);
        //        //    AnalyticsService.ReportException(ex, message);
        //        //    retry = false;
        //        //    throw new InvalidUsernameOrPasswordException("Authentication failure - please check username and password");
        //        //}
        //        catch (WebException ex)
        //        {
        //            //code for other WebException
        //            string message = string.Format("Received unexpected response attempting to authenticate user: '{0}' with response: {1}. Login attempt: {2}", _username, ex.Response, retryAttempt);
        //            _analyticsService.ReportException(ex, message);
        //        }
        //        catch (Exception ex)
        //        {
        //            // catch all others...
        //            string message = string.Format("Received an unexpected exception when attempting to authenticate user: '{0}'. Login attempt: {1}", _username, retryAttempt);
        //            _analyticsService.ReportException(ex, message);
        //        }
        //        finally
        //        {
        //            retryAttempt++;
        //            retry = retry && (retryAttempt < _retryCount);
        //            if (retry)
        //            {
        //                System.Threading.Thread.Sleep(1000); // wait 1 second before attempting again...
        //            }
        //            else
        //            {
        //                credentialCheck = false; // set this to false to ensure we can exit the while condition
        //            }
        //        }
        //    }
        //}

        //internal async Task<string> GetAccessToken(bool credsCheck = false)
        //{
        //    string[] scopes = GetDefaultScope();
        //    string accessToken = await GetAccessTokenAsync(scopes, credsCheck);
        //    return accessToken;
        //}

        ///// <summary>
        ///// Returns an access token for the given scopes.
        ///// </summary>
        ///// <param name="scopes">The scopes to retrieve the access token for</param>
        ///// <param name="prompt">The prompt style to use. Notice that this only works with the Interactive Login flow, for all other flows this parameter is ignored.</param>
        ///// <returns></returns>
        //public async Task<string> GetAccessTokenAsync(string[] scopes, bool credsCheck = false)
        //{
        //    return await GetAccessTokenAsync(scopes, CancellationToken.None, credsCheck);
        //}

        ///// <summary>
        ///// Returns an access token for the given scopes.
        ///// </summary>
        ///// <param name="scopes">The scopes to retrieve the access token for</param>
        ///// <param name="cancellationToken">Optional cancellation token to cancel the request</param>
        ///// <param name="prompt">The prompt style to use. Notice that this only works with the Interactive Login flow, for all other flows this parameter is ignored.</param>
        ///// <returns></returns>
        //public async Task<string> GetAccessTokenAsync(string[] scopes, CancellationToken cancellationToken, bool credsCheck = false)
        //{
        //    AuthenticationResult authResult = null;
        //    if (credsCheck)
        //    {
        //        authResult = await GetNewAccessTokenAsync(scopes, cancellationToken);
        //    }
        //    else
        //    {
        //        try
        //        {
        //            authResult = await GetCachedAccessTokenAsync(scopes, cancellationToken);
        //        }
        //        catch (MsalUiRequiredException)
        //        {
        //            try
        //            {
        //                // This means we need to login again through the MSAL window.
        //                authResult = await GetNewAccessTokenAsync(scopes, cancellationToken);
        //            }
        //            catch (Exception ex)
        //            {
        //                Debug.WriteLine(ex.ToString());
        //                return null;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // Log Error....
        //            authResult = await GetNewAccessTokenAsync(scopes, cancellationToken);
        //        }
        //    }
        //    if (authResult?.AccessToken != null)
        //    {
        //        return authResult.AccessToken;
        //    }
        //    return null;
        //}

        //public async Task<AuthenticationResult> GetCachedAccessTokenAsync(string[] scopes, CancellationToken cancellationToken, bool throwOnError = true)
        //{
        //    var accounts = await PublicClientApplication.GetAccountsAsync();
        //    try
        //    {
        //        return await PublicClientApplication.AcquireTokenSilent(scopes, accounts.First()).ExecuteAsync(cancellationToken);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (!throwOnError) return null;
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// Explicitly requests a new access token for the given scopes from the server (not the cache)
        ///// </summary>
        ///// <param name="scopes"></param>
        ///// <param name="cancellationToken"></param>
        ///// <returns></returns>
        //public async Task<AuthenticationResult> GetNewAccessTokenAsync(string[] scopes, CancellationToken cancellationToken)
        //{
        //    AuthenticationResult authResult = null;
        //    try
        //    {
        //        // This means we need to login again through the MSAL window.
        //        authResult = await PublicClientApplication.AcquireTokenInteractive(scopes)
        //                                    .WithParentActivityOrWindow(ParentWindow)
        //                                    .WithUseEmbeddedWebView(true)
        //                                    .ExecuteAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //    }
        //    return authResult;
        //}

        ///// <summary>
        ///// Clears the internal in-memory token cache used by MSAL
        ///// </summary>
        //public void ClearTokenCache()
        //{
        //    AsyncHelper.RunSync(ClearTokenCacheAsync);
        //}

        ///// <summary>
        ///// Clears the internal in-memory token cache used by MSAL
        ///// </summary>
        //public async Task ClearTokenCacheAsync()
        //{
        //    if (PublicClientApplication == null) return;

        //    var accounts = (await PublicClientApplication.GetAccountsAsync()).ToList();
        //    while (accounts.Any())
        //    {
        //        await PublicClientApplication.RemoveAsync(accounts.First());
        //        accounts = (await PublicClientApplication.GetAccountsAsync()).ToList();
        //    }
        //}

        //internal string[] GetDefaultScope()
        //{
        //    string baseScope = HttpUtility.ConcatUrls(Tenant, ".default");
        //    string[] scopes = { baseScope };
        //    return scopes;
        //}

        //ValueTask IAuthenticator.Authenticate(IRestClient client, RestRequest request)
        //{
        //    throw new NotImplementedException();
        //}

        #endregion old Xamarin app's code





        #region Old ws-trust code - Deprecated

        // Code below is largely depracted - it was based on using the WS-Trust method of authentication for which support will be discontinued by Microsoft in late 2021
        // https://docs.microsoft.com/en-us/power-platform/important-changes-coming#deprecation-of-office365-authentication-type-and-organizationserviceproxy-class-for-connecting-to-common-data-service

        /// The approach laid out in this class is based off of a blog post regarding custom authentication to SharePointOnline:
        /// https://blogs.technet.microsoft.com/sharepointdevelopersupport/2018/02/07/sharepoint-online-active-authentication/
        /// in addition to creating a simple Console Application (.Net 4.6.1) that uses the CSOM libraries to connect to SP and watching the generated http traffic
        /// Note, once CSOM libraries are built and available for .Net standard projects - they can be imported and used instead of this 


        //private string GetUserRealmUrl()
        //{
        //    var responseBody = HttpUtility.GetResponseBody(_getRealmUrl, string.Format("login={0}&xml=1", _username), null, "POST", null, null);
        //    return GetCustomSTSAuthUrl(responseBody);
        //}

        //internal string RequestBinarySecurityToken()
        //{
        //    string customStsAuthUrl = GetUserRealmUrl();
        //    string responseString;
        //    string templateBody;
        //    string body;

        //    if (string.IsNullOrEmpty(customStsAuthUrl))
        //    {
        //        templateBody = GetLocalStringResource("Data.SharePoint.Authentication.MSOOnlySamlRequest.xml");
        //        body = string.Format(templateBody, MSODomain, _username, Utility.EncodeString(_password.ToInsecureString()));
        //    }
        //    else
        //    {
        //        string customStsAssertion = GetAssertionCustomSts(customStsAuthUrl);
        //        templateBody = GetLocalStringResource("Data.SharePoint.Authentication.STSandMSOSamlRequest.xml");
        //        body = string.Format(templateBody, customStsAssertion, MSODomain);
        //    }
        //    responseString = HttpUtility.GetResponseBody(_msoStsAuthUrl, body, null, "POST", null, null);
        //    var token = GetBinarySecurityToken(responseString);
        //    if (!string.IsNullOrEmpty(token)) { return token; }

        //    // Otherwise there must have been an issue in attempting to retrieve the token
        //    // Handle this - check for error messages in response string, attempt to give a targeted exception
        //    string reason = GetFailureMessage(responseString);
        //    if (reason == "Authentication Failure")
        //    {
        //        string details = GetFailureMessageDetail(responseString);
        //        if (details.Contains("User account is disabled"))
        //        {
        //            throw new AccountDisabledException("Failed to successfully retrieve binary security token - due to account being disabled");
        //        }
        //        if (details.Contains("Invalid username or password"))
        //        {
        //            throw new InvalidUsernameOrPasswordException("Failed to successfully retrieve binary security token - invalid username or password");
        //        }
        //    }

        //    // Otherwise there is some other reason why we have not successfully retrieved the binary security token
        //    throw new InvalidUsernameOrPasswordException("Failed to successfully retrieve binary security token - check account details and access");
        //}

        ///// <summary>
        ///// Gets the assertion custom STS. 
        ///// Makes a seurity token request to the corporate ADFS proxy usernamemixed endpoint using 
        ///// the user's corporate credentials. The logon token is used to talk to MSO STS to get 
        ///// an O365 service token that can then be used to sign into SPO. 
        ///// </summary>
        ///// <param name="customStsAuthUrl">The custom STS authentication URL.</param>
        ///// <returns></returns>
        //internal string GetAssertionCustomSts(string customStsAuthUrl)
        //{
        //    try
        //    {
        //        Guid messageId = Guid.NewGuid();
        //        string created = DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture);
        //        string expires = DateTime.UtcNow.AddMinutes(10).ToString("o", System.Globalization.CultureInfo.InvariantCulture);
        //        string requestSecurityToken;

        //        if (_integrated)
        //        {
        //            customStsAuthUrl = customStsAuthUrl.ToLowerInvariant().Replace("/usernamemixed", "/windowstransport");
        //            string customStsSamlIntegratedRequestFormat = GetLocalStringResource("Data.SharePoint.Authentication.CustomStsSamlIntegratedRequestFormat.xml");
        //            requestSecurityToken = string.Format(customStsSamlIntegratedRequestFormat, messageId, customStsAuthUrl, _realm);
        //        }
        //        else
        //        {
        //            string customStsSamlRequestFormat = GetLocalStringResource("Data.SharePoint.Authentication.CustomStsSamlRequestFormat.xml");
        //            requestSecurityToken = string.Format(customStsSamlRequestFormat, customStsAuthUrl, messageId, _username, Utility.EncodeString(_password.ToInsecureString()), created, expires, _realm);
        //        }

        //        var headers = new Dictionary<string, string>();
        //        headers.Add("Content-Type", "application/soap+xml;charset=utf-8");
        //        string responseString = HttpUtility.GetResponseBody(customStsAuthUrl, requestSecurityToken, headers, "POST", null, null);

        //        // convert string to stream
        //        byte[] byteArray = Encoding.UTF8.GetBytes(responseString);
        //        using (MemoryStream memStream = new MemoryStream(byteArray))
        //        {
        //            using (StreamReader stream = new StreamReader(memStream))
        //            {
        //                XPathNavigator nav = new XPathDocument(stream).CreateNavigator();
        //                XPathNodeIterator nodes = nav.Select("/bookstore/book");
        //                XmlNamespaceManager nsMgr = new XmlNamespaceManager(nav.NameTable);
        //                nsMgr.AddNamespace("t", "http://schemas.xmlsoap.org/ws/2005/02/trust");
        //                XPathNavigator requestedSecurityToken = nav.SelectSingleNode("//t:RequestedSecurityToken", nsMgr);

        //                // Ensure whitespace is reserved 
        //                XmlDocument doc = new XmlDocument();
        //                doc.LoadXml(requestedSecurityToken.InnerXml);
        //                doc.PreserveWhitespace = true;
        //                return doc.InnerXml;
        //            }
        //        }
        //    }
        //    catch (WebException ex)
        //    {
        //        if (ex.Response == null) throw;
        //        string exMessage;
        //        using (StreamReader responseReader = new StreamReader(ex.Response.GetResponseStream()))
        //        {
        //            exMessage = responseReader.ReadToEnd();
        //            string failReason = GetFailureMessage(exMessage);
        //            if (failReason.Contains("The security token could not be authenticated or authorized"))
        //            {
        //                throw new InvalidUsernameOrPasswordException(failReason);
        //            }
        //        }
        //        throw;
        //    }
        //    catch (Exception ex)
        //    {
        //        // log exception...
        //        AnalyticsService.ReportException(ex, "There was an issue getting the custom assertion sts url");
        //        throw;
        //    }
        //}

        ///// <summary>
        ///// Gets the custom STS authentication URL - Retrieve the configured STS Auth Url (ADFS, PING, etc.)
        ///// If we are using standard MS Auth process there will be no STS Auth Url.
        ///// </summary>
        ///// <param name="responseString">The response string.</param>
        ///// <returns></returns>
        //internal static string GetCustomSTSAuthUrl(string responseString)
        //{
        //    try
        //    {
        //        XmlDocument xmldoc = new XmlDocument();
        //        xmldoc.LoadXml(responseString);
        //        var customAuthNode = xmldoc.SelectSingleNode("RealmInfo/STSAuthURL");
        //        if (customAuthNode == null)
        //        {
        //            return string.Empty;
        //        }
        //        return customAuthNode.InnerText;
        //    }
        //    catch
        //    {
        //        return string.Empty;
        //    }
        //}

        //internal static string GetBinarySecurityToken(string responseString)
        //{
        //    // Get binary security token using regex instead of [xml]
        //    Regex rx = new Regex(@"BinarySecurityToken Id=.*>([^<]+)<", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //    var match = rx.Match(responseString).Groups[1];
        //    return match.Value;
        //}

        //// Alternate method to get BinarySecurityToken if required
        //internal static string GetBinarySecurityTokenXPath(string responseString)
        //{
        //    var xData = XDocument.Parse(responseString);
        //    var namespaceManager = new XmlNamespaceManager(new NameTable());
        //    namespaceManager.AddNamespace("S", "http://www.w3.org/2003/05/soap-envelope");
        //    namespaceManager.AddNamespace("wst", "http://schemas.xmlsoap.org/ws/2005/02/trust");
        //    namespaceManager.AddNamespace("wsse", "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd");
        //    var binarySecurityToken = xData.XPathSelectElement("/S:Envelope/S:Body/wst:RequestSecurityTokenResponse/wst:RequestedSecurityToken/wsse:BinarySecurityToken", namespaceManager);
        //    return binarySecurityToken.Value;
        //}

        //internal static string GetFailureMessage(string responseString)
        //{
        //    var xData = XDocument.Parse(responseString);
        //    var namespaceManager = new XmlNamespaceManager(new NameTable());
        //    namespaceManager.AddNamespace("S", "http://www.w3.org/2003/05/soap-envelope");
        //    namespaceManager.AddNamespace("psf", "http://schemas.microsoft.com/Passport/SoapServices/SOAPFault");
        //    var reason = xData.XPathSelectElement("/S:Envelope/S:Body/S:Fault/S:Reason/S:Text", namespaceManager);
        //    return reason.Value;
        //}

        //internal static string GetFailureMessageDetail(string responseString)
        //{
        //    var xData = XDocument.Parse(responseString);
        //    var namespaceManager = new XmlNamespaceManager(new NameTable());
        //    namespaceManager.AddNamespace("S", "http://www.w3.org/2003/05/soap-envelope");
        //    namespaceManager.AddNamespace("psf", "http://schemas.microsoft.com/Passport/SoapServices/SOAPFault");
        //    var reason = xData.XPathSelectElement("/S:Envelope/S:Body/S:Fault/S:Detail/psf:error/psf:internalerror/psf:text", namespaceManager);
        //    return reason.Value;
        //}

        //internal static Cookie GetSPOIDCRLCookie(string spTenantUrl, string msoBinarySecurityToken)
        //{
        //    string msoBinarySecurityTokenHeader = string.Format("BPOSIDCRL {0}", msoBinarySecurityToken);
        //    var headers = new Dictionary<string, string>();
        //    headers.Add("Authorization", msoBinarySecurityTokenHeader);
        //    headers.Add("X-IDCRL_ACCEPTED", "t");
        //    return HttpUtility.GetResponseCookie(string.Format("{0}/_vti_bin/idcrl.svc/", spTenantUrl), string.Empty, headers, "GET", null, _spAuthCookieName);
        //}

        //internal static string GetDigest(string url, CookieContainer cookies)
        //{
        //    string requestBody = GetLocalStringResource("Data.SharePoint.Authentication.GetUpdatedDigestInfo.xml");
        //    string requestUrl = HttpUtility.ConcatUrls(url, _contextInfoPath);
        //    string responseBody = HttpUtility.GetResponseBody(requestUrl, requestBody, null, "POST", null, cookies);
        //    XElement digestResponse = XElement.Parse(responseBody);
        //    return digestResponse.Descendants().Where(x => x.Name.LocalName == "FormDigestValue").First().Value;
        //}

        ///// <summary>
        ///// Loads up an embedded resource and returns it as a string
        ///// </summary>
        ///// <param name="resourcePath"></param>
        //internal static string GetLocalStringResource(string resourcePath)
        //{
        //    var assembly = Assembly.GetExecutingAssembly();
        //    string stringResource;
        //    using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
        //    {
        //        using (StreamReader reader = new StreamReader(stream))
        //        {
        //            stringResource = reader.ReadToEnd();
        //        }
        //    }
        //    return stringResource;
        //}

        #endregion

    }
}


