using Core;
using Core.Analytics;
using Core.Helpers;
using Core.Interfaces;
using Core.Models;
using Data.SharePoint.Authentication;
using Data.SharePoint.Clients;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Data.SharePoint.Services
{
    public class ClientManager : IClientManager
    {
        HttpClient _client = null;
        private readonly HttpClientHandler _clientHandler;
        private readonly CookieContainer _cookieContainer;
        private readonly string _siteUrl = Consts.SiteUrl;
        private readonly IAnalyticsService _analyticsService;
        private readonly IHttpUtility _httpUtility;
        public string ListName { get; set; }
        public string ODataQuery { get; set; }

        public ClientManager(
            IAnalyticsService analyticsService,
            IHttpUtility httpUtility)
        {
            _analyticsService = analyticsService;
            _httpUtility = httpUtility;
            _cookieContainer = new CookieContainer();
            _clientHandler = new HttpClientHandler() { CookieContainer = _cookieContainer };
        }

        public async Task EnsureClient()
        {
            if (_client == null)
            {

#if DEBUG
                // _clientHandler.Proxy = new WebProxy("http://192.168.50.140:8888");
#endif
                _client = new HttpClient(_clientHandler);

                // TODO: need to handle if no access token returned here. Might need to return user to Login page...
                var accessToken = await SPAuthenticator.Instance.GetAccessToken(false, true); // get cached access token
                _client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            }
        }

        // Consolidates GetAll and GetByQuery into a single flexible method
        public async Task<List<T>> GetAllItems<T>(string listName, string odataQuery = null)
        {
            ListName = listName;
            ODataQuery = odataQuery;
            string url = _httpUtility.ConcatUrls(_siteUrl, $"_api/web/lists/getbytitle('{listName}')/Items{odataQuery ?? string.Empty}");
            await EnsureClient();
            var request = CreateRequest(url, HttpMethod.Get);
            try
            {
                var response = await _client.SendAsync(request).ConfigureAwait(false);
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<SpJsonData<T>>(jsonResponse);
                    //return data?["value"] ?? new List<T>();
                    return data?.value ?? new List<T>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }
        public async Task<T> GetSingleItem<T>(string listName, int id) where T : class
        {
            string filter = $"$filter=Id eq {id}";
            string url = _httpUtility.ConcatUrls(_siteUrl, $"_api/web/lists/getbytitle('{listName}')/Items?{filter}");
            await EnsureClient();
            var request = CreateRequest(url, HttpMethod.Get);
            var response = await _client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<Dictionary<string, List<T>>>(jsonResponse);

                if (data != null && data["value"] != null && data.ContainsKey("value") && data["value"].Count > 0)
                {
                    return data["value"].FirstOrDefault();
                }
            }
            return default(T);
        }

        public async Task<IList<T>> GetItemsByCustomUrl<T>(string customUrl)
        {
            try
            {
                await EnsureClient();
                var request = CreateRequest(customUrl, HttpMethod.Get);
                var response = await _client.SendAsync(request);

                Console.WriteLine($"Response Status Code: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<SpJsonData<T>>(jsonResponse);
                    return data.value;
                }
                return default(IList<T>);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return default(IList<T>);
            }
        }

        public async Task<T> GetItemByCustomUrl<T>(string customUrl)
        {
            try
            {
                await EnsureClient();
                var request = CreateRequest(customUrl, HttpMethod.Get);
                var response = await _client.SendAsync(request);

                Console.WriteLine($"Response Status Code: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<T>(jsonResponse);
                    return data;
                }
                return default(T);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return default(T);
            }
        }
        public async Task<string> GetListItemEntityTypeFullName(string listName)
        {
            string url = _httpUtility.ConcatUrls(_siteUrl, $"_api/web/lists/getbytitle('{listName}')/listItemEntityTypeFullName");
            await EnsureClient();
            var request = CreateRequest(url, HttpMethod.Get);
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonResponse);
                return data?["value"] ?? string.Empty;
            }
            return string.Empty;
        }

        public async Task<int> GetCount(string listName, string odataQuery = null)
        {
            string url = _httpUtility.ConcatUrls(_siteUrl, $"_api/web/lists/getbytitle('{listName}')/ItemCount{odataQuery ?? string.Empty}");
            await EnsureClient();
            var request = CreateRequest(url, HttpMethod.Get);
            var response = await _client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonResponse);
                return data?["ItemCount"] ?? 0;
            }
            return 0;
        }

        public async Task<T> CreateItem<T>(string listName, object body = null)
        {
            string url = _httpUtility.ConcatUrls(_siteUrl, $"_api/web/lists/getbytitle('{listName}')/items");
            await EnsureClient();
            var request = CreateRequest(url, HttpMethod.Post, body);
            HttpResponseMessage response;
            try
            {
                response = await _client.SendAsync(request);
                if (response == null)
                {
                    throw new HttpRequestException("Failed to submit YourSay form - request failed");
                }
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(string.Format("Failed to submit YourSay request - result: {0}", response.StatusCode));
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Failed (Unexpected) to submit YourSay request: {0}, {1}", ex, ex.Message);
                var properties = new Dictionary<string, string> { { "Message", message }, { "ExceptionMessage", ex.Message }, { "ExceptionSource", ex.Source }, { "StackTrace", ex.StackTrace } };
                _analyticsService.ReportException(ex, properties);
                throw;
            }

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var data = JsonConvert.DeserializeObject<SpJsonDataVerbose<T>>(jsonResponse);

                if (data == null || data.d == null)
                {
                    throw new Exception($"Received null value back from attempting to save YourSay form to SharePoint");
                }
                return data.d;
            }
            else
            {
                throw new Exception($"Received unexpected status code: {response.StatusCode}");
            }
        }

        public async Task<int> UpdateItem(string listName, string odataQuery = null)
        {
            // TODO
            return 0;
        }

        public async Task<bool> UploadAttachment(string listName, int itemId, byte[] attachmentBytes, string attachmentName)
        {
            string url = _httpUtility.ConcatUrls(_siteUrl, $"_api/web/lists/GetByTitle('{listName}')/items({itemId})/AttachmentFiles/add(FileName='{attachmentName}')");
            await EnsureClient();
            var request = CreateRequest(url, HttpMethod.Post, attachmentBytes);
            HttpResponseMessage response;
            try
            {
                response = await _client.SendAsync(request);
                if (response == null)
                {
                    throw new HttpRequestException("Failed to upload attachment for your say form - request failed");
                }
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException(string.Format("Failed to upload attachment for YourSay request - result: {0}", response.StatusCode));
                }
            }
            catch (Exception ex)
            {
                string message = string.Format("Failed (Unexpected) to upload attachment for YourSay request: {0}, {1}", ex, ex.Message);
                var properties = new Dictionary<string, string> { { "Message", message }, { "ExceptionMessage", ex.Message }, { "ExceptionSource", ex.Source }, { "StackTrace", ex.StackTrace } };
                _analyticsService.ReportException(ex, properties);
                throw;
            }

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            return false;
        }

        public async Task<bool> DownloadFile(string url, string saveLocation)
        {
            var baseSPUrl = HttpUtility.GetHostFromFullUrl(Consts.SiteUrl);
            var siteRelativeUrl = url.Replace(baseSPUrl, string.Empty);

            try
            {
                string downloadUrl = url;
                if (url.Contains(Consts.SiteUrl))
                {
                    //var downloadUrl = Consts.SiteUrl + "/_api/web/getfilebyserverrelativeurl('" + url.Replace(Consts.SiteUrl, string.Empty) + "')/$value"; //url.Replace(baseSPUrl, string.Empty);
                    //var downloadUrl = "https://yarratrams.sharepoint.com/sites/staffapp/_api/web/getfilebyserverrelativeurl('/sites/staffapp/news/PublishingImages/Lists/News/EditForm/Priceline%20FAQs.pdf')/$value";
                    downloadUrl = $"{Consts.SiteUrl}_api/web/getfilebyserverrelativeurl('{siteRelativeUrl}')/$value";
                }
                else if (url.Contains(baseSPUrl)) // elsewhere in SharePoint...
                {
                    var siteUrl = HttpUtility.GetSPSitePathFromFullUrl(url);
                    downloadUrl = $"{siteUrl}/_api/web/getfilebyserverrelativeurl('{siteRelativeUrl}')/$value";
                }
                await EnsureClient();
                var request = CreateRequest(downloadUrl, HttpMethod.Get);
                var response = await _client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var downloadBytes = await response.Content.ReadAsByteArrayAsync();
                    await File.WriteAllBytesAsync(saveLocation, downloadBytes);
                }
                return File.Exists(saveLocation);
            }
            catch (Exception ex)
            {
                await _analyticsService.ReportException(ex);
                Debug.WriteLine(ex);
                return false;
            }
        }

        #region Helper Methods
        internal static HttpRequestMessage CreateRequest(string url, HttpMethod method, object body = null)
        {
            var uri = new Uri(url, UriKind.Absolute);
            var request = new HttpRequestMessage(method, uri);

            SetContentType(method, request);
            SetBody(body, request);
            return request;
        }

        internal static void SetContentType(HttpMethod method, HttpRequestMessage request)
        {
            try
            {
                if (method == HttpMethod.Post || method == HttpMethod.Put)
                {
                    request.Headers.Add("Accept", "application/json;odata=verbose");
                }
                if (method == HttpMethod.Get)
                {
                    request.Headers.Add("Accept", "application/json;odata=nometadata");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        internal static void SetBody(object body, HttpRequestMessage request)
        {
            if (body == null) return;

            if (body.GetType() == typeof(byte[]))
            {
                request.Content = new ByteArrayContent((byte[])body);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            }
            else
            {
                var json = JsonConvert.SerializeObject(body);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                request.Content = stringContent;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                request.Content.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("odata", "verbose"));
            }
        }
        #endregion

    }
}
