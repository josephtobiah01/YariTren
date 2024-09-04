using Data.SharePoint.Authentication;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security;
using System.Text;
using Core.Helpers;
using Core.Models;
using RestSharp.Authenticators;
using System.Diagnostics;
using Core.Analytics;
using Core.Interfaces;

namespace Data.SharePoint.Clients
{
    /// <summary>
    /// Base method for calling SharePoint rest api.  Includes methods for GetAll, and GetCount.
    /// These methods can be overriden if required, but most of the time to create a new rest client to retrieve data from a list you will just have to:
    /// 1) Create a new class that inherits off of ClientBase
    /// 2) Set value for ListName (Required)
    /// 3) Set value for ODataQuery (Optional)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="Data.SharePoint.Clients.IClient{T}" />
    public class ClientBase<T> : IClient<T>
    {
        protected  string _baseUrl;
        protected string _siteUrl;
        private string ListRestApiItems = "/_api/web/lists/getbytitle('{0}')/Items{1}";
        private string ListRestApiCount = "/_api/web/lists/getbytitle('{0}'){1}";
        private static SPAuthenticator _authenticator;
        private readonly IAnalyticsService _analyticsService;

        #region Properties

        protected string ListName { get; set; }
        protected string ODataQuery { get; set; }
        protected string GetSingleODataQuery { get; set; } = "$filter=Id eq '{0}'";

        protected string GetListItemsUrl
        {
            get
            {
                return HttpUtility.ConcatUrls(_siteUrl, string.Format(ListRestApiItems, ListName, ODataQuery));
            }
        }

        protected string GetSingeListItemUrl
        {
            get
            {
                return HttpUtility.ConcatUrls(_siteUrl, string.Format(ListRestApiItems, ListName, GetSingleODataQuery));
            }
        }

        protected string GetListItemEntityTypeFullNameUrl
        {
            get
            {
                return HttpUtility.ConcatUrls(_siteUrl, string.Format("/_api/web/lists/getbytitle('{0}')/listItemEntityTypeFullName", ListName));
            }
        }

        protected string GetListItemCountUrl
        {
            get
            {
                return HttpUtility.ConcatUrls(_siteUrl, string.Format(ListRestApiCount, ListName, ODataQuery));
            }
        }

        protected RestClient RestClient { get; set; }

        #endregion

       


        /// <summary>
        /// Initializes a new instance of the <see cref="ClientBase{T}"/> class.
        /// </summary>
        /// <param name="siteUrl">The site URL, including the path to the subsite if this is the vase</param>
        /// <param name="userName">Username</param>
        /// <param name="password">Password</param>
        //public ClientBase(string siteUrl, string userName, SecureString password)
        //{
        //    _siteUrl = siteUrl;
        //    _baseUrl = HttpUtility.GetHostFromFullUrl(siteUrl);
        //    Initialise(userName, password);
        //}

        public ClientBase(
            string siteUrl, 
            IAuthenticator authenticator,
            IAnalyticsService analyticsService)
        {
            _siteUrl = siteUrl;
            _baseUrl = HttpUtility.GetHostFromFullUrl(siteUrl);
            _authenticator = authenticator as SPAuthenticator;
            _analyticsService = analyticsService;

            Initialise();

            // Use for testing with Fiddler if required:
//#if DEBUG
//            RestClient.Proxy = new WebProxy("http://192.168.65.140:8888");
//#endif
        }

        /// <summary>
        /// Override in descendents if necessary
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public virtual IList<T> GetAll()
        {
            string url = GetListItemsUrl;
            RestRequest request = CreateRequest(url, Method.Get, null);

            var result = RestClient.Execute<SpJsonData<T>>(request);
            if (result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.Created)
            {
                return result.Data.value;
            }
            return null;
        }

        public virtual IList<T> GetSingleItem()
        {
            string url = GetSingeListItemUrl;
            RestRequest request = CreateRequest(url, Method.Get, null);

            var result = RestClient.Execute<SpJsonData<T>>(request);
            if (result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.Created)
            {
                return result.Data.value;
            }
            return null;
        }

        public virtual IList<T> GetSingleItemForUpdate()
        {
            string url = GetSingeListItemUrl;
            RestRequest request = CreateRequest(url, Method.Get, null);
            request.AddHeader("Accept", "application/json; odata=verbose;");

            var result = RestClient.Execute<SpJsonDataVerbose<SpJsonDataResults<List<T>>>>(request);
            if (result.IsSuccessful && result.Data != null && result.Data.d != null)
            {
                return result.Data.d.results ?? null;
            }
            return null;
        }

        /// <summary>
        /// Override in descendents if necessary
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public virtual int GetCount()
        {
            string url = GetListItemCountUrl;
            RestRequest request = CreateRequest(url, Method.Get, null);

            // Use for testing with Fiddler if required:
            #if DEBUG
            // RestClient.Proxy = new WebProxy("http://localhost:8888");
            #endif

            var result = RestClient.Execute<SPItemCount>(request);
            if (result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.Created)
            {
                return result.Data.ItemCount;
            }
            return 0;
        }

        public string GetListItemEntityTypeFullName()
        {
            string url = GetListItemEntityTypeFullNameUrl;
            RestRequest request = CreateRequest(url, Method.Get, null);

            var result = RestClient.Execute<SPJsonSingleVal>(request);
            if (result.StatusCode == HttpStatusCode.OK || result.StatusCode == HttpStatusCode.Created)
            {
                return result.Data.value;
            }
            return null;
        }

        public bool DownloadFile(string url, string saveLocation)
        {
            RestRequest request = CreateRequest(url, Method.Get, null);
            try
            {
                byte[] downloadBytes = RestClient.DownloadData(request);
                File.WriteAllBytes(saveLocation, downloadBytes);
                return true;
            }
            catch(Exception ex)
            {
                string message = string.Format("Error downloading/saving pdf file to '{0}' from URL: {1}", saveLocation, url);
                var exceptionMethodParent = new StackFrame(1).GetMethod().Name;
                var properties = new Dictionary<string, string> { { "ExceptionMethodParent", exceptionMethodParent }, { "Message", message } };
                _analyticsService.ReportException(ex, properties);
                return false;
            }
        }

        #region Helper Methods

        internal RestRequest CreateRequest(string url, Method method, object body = null)
        {
            var uri = new Uri(url,UriKind.Absolute);
            var request = new RestRequest(uri, method);
            SetContentType(method, request);
            SetBody(body, request);
            return request;
        }


        private static void SetContentType(Method method, RestRequest request)
        {
            if (method == Method.Post || method == Method.Put)
            {
                request.AddHeader("Content-Type", "application/json;odata=verbose");
                request.AddHeader("Accept", "application/json;odata=verbose");
            }
            if (method == Method.Get)
            {
                request.AddHeader("Accept", "application/json; odata=nometadata;");
            }
            request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json;odata=verbose"; };
        }

        private static void SetBody(object body, RestRequest request)
        {
            if (body != null)
            {
                request.AddJsonBody(body);
            }
        }

        private void Initialise()
        {
            try
            {
                RestClient = new RestClient(_baseUrl);

                var option = new RestClientOptions(_baseUrl)
                {
                    Authenticator = _authenticator,
                    CookieContainer = new System.Net.CookieContainer()
                };
                // Temporary - for debugging:
                // RestClient.Proxy = new WebProxy("192.168.65.140:8888");
            }
            catch(Exception)
            {
                //string message = $"Error attempting to connect to: ${_baseUrl}";
                //var exceptionMethodParent = new StackFrame(1).GetMethod().Name;
                //var properties = new Dictionary<string, string> { { "ExceptionMethodParent", exceptionMethodParent }, { "Message", message } };
                //AnalyticsService.ReportException(ex, properties);
                throw;
            }
        }
    }
    #endregion
}
