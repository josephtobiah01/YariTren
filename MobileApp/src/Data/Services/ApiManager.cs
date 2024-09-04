using Core.Interfaces;
using Data.SharePoint.Authentication;
using Microsoft.Maui.Networking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NetworkAccess = Microsoft.Maui.Networking.NetworkAccess;

namespace Data.Services
{
    public class ApiManager : IApiManager
    {

        //private string baseUrl = "http://192.168.1.7:5027/";
        HttpClient _client = null;
        private readonly HttpClientHandler _clientHandler;
        private readonly CookieContainer _cookieContainer;



        public ApiManager()
        {
            //_client.BaseAddress = new Uri(baseUrl);
            _cookieContainer = new CookieContainer();
            _clientHandler = new HttpClientHandler() { CookieContainer = _cookieContainer };
            _client = new HttpClient(_clientHandler);
        }

        public void SetBaseUrl(string baseUrl)
        {
            _client.BaseAddress = new Uri(baseUrl);
        }

        public async Task InitializeClient()
        {
            if (_client == null)
            {
                _client = new HttpClient();
            }

            _client.DefaultRequestHeaders.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var accessToken = await SPAuthenticator.Instance.GetAccessToken(false, true);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            Console.WriteLine("DefaultRequestHeaders:");
            foreach (var header in _client.DefaultRequestHeaders)
            {
                Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }
        }

        //private void SetHeaders(HttpRequestMessage request, HttpMethod method, HttpContent content = null)
        //{
        //    try
        //    {
        //        if (method == HttpMethod.Get)
        //        {
        //            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        //        }
        //        else if (method == HttpMethod.Post || method == HttpMethod.Put)
        //        {
        //            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json;odata=verbose"));
        //            if (content != null)
        //            {
        //                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //            }
        //        }
        //    }
        //    catch (Exception ex) 
        //    {
        //        Console.WriteLine(ex.ToString());
        //    }
            
        //}

        public async Task<T> SendForResponseAsync<T>(string uriRequest, HttpMethod method, HttpContent content, CancellationToken cancellationToken = default)
        {
            T response = default(T);

            await InitializeClient();

            NetworkAccess accessType = Connectivity.Current.NetworkAccess;

            if (accessType == NetworkAccess.Internet)
            {
                var request = new HttpRequestMessage(method, uriRequest) { Content = content };
                //SetHeaders(request, method, content);

                if (content != null && (method == HttpMethod.Post || method == HttpMethod.Put))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                }

                try
                {
                    HttpResponseMessage httpResponse = await _client.SendAsync(request, cancellationToken);

                    if (httpResponse.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception($"Request failed with status code: {httpResponse.StatusCode} and reason: {httpResponse.ReasonPhrase}");
                    }

                    if (httpResponse.IsSuccessStatusCode)
                    {
                        var res = await httpResponse.Content.ReadAsStringAsync();
                        response = JsonConvert.DeserializeObject<T>(res);
                    }
                }
                catch (Exception ex) 
                {
                    Console.WriteLine(ex.ToString());
                }
                
            }
            else
            {
                throw new Exception("There is a problem with your internet connection");
            }

            return response;
        }

        public async Task<T> PostForResponseAsync<T>(string uriRequest, HttpContent content, CancellationToken cancellationToken = default)
        {
            return await SendForResponseAsync<T>(uriRequest, HttpMethod.Post, content, cancellationToken);
        }

        public async Task<T> GetForResponseAsync<T>(string uriRequest, CancellationToken cancellationToken = default)
        {
            return await SendForResponseAsync<T>(uriRequest, HttpMethod.Get, null, cancellationToken);
        }

        public async Task<T> PutForResponseAsync<T>(string uriRequest, HttpContent content, CancellationToken cancellationToken = default)
        {
            return await SendForResponseAsync<T>(uriRequest, HttpMethod.Put, content, cancellationToken);
        }

        public async Task<T> DeleteForResponseAsync<T>(string uriRequest, CancellationToken cancellationToken = default)
        {
            return await SendForResponseAsync<T>(uriRequest, HttpMethod.Delete, null, cancellationToken);
        }


        //public async Task<T> PostForResponseAsync<T>(string uriRequest, StringContent content, CancellationToken cancellationToken = default)
        //{
        //    T response = default(T);

        //    NetworkAccess accessType = Connectivity.Current.NetworkAccess;

        //    if (accessType == NetworkAccess.Internet)
        //    {

        //        HttpResponseMessage httpResponse = await _client.PostAsync(uriRequest, content, cancellationToken);


        //        if (httpResponse.IsSuccessStatusCode)
        //        {
        //            var res = await httpResponse.Content.ReadAsStringAsync();
        //            response = JsonConvert.DeserializeObject<T>(res);
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception("Ooops, there is a problem with your internet connection");
        //    }

        //    return response;
        //}

        //public async Task<T> GetForResponseAsync<T>(string uriRequest, CancellationToken cancellationToken = default)
        //{
        //    T? response = default;

        //    NetworkAccess accessType = Connectivity.Current.NetworkAccess;

        //    if (accessType == NetworkAccess.Internet)
        //    {

        //        HttpResponseMessage httpResponse = await _client.GetAsync(uriRequest, cancellationToken);


        //        if (httpResponse.IsSuccessStatusCode)
        //        {
        //            var res = await httpResponse.Content.ReadAsStringAsync();
        //            response = JsonConvert.DeserializeObject<T>(res);
        //        }

        //    }
        //    else
        //    {
        //        throw new Exception("Ooops, there is a problem with your internet connection");
        //    }

        //    return response;
        //}

        //public async Task<T> PutForResponseAsync<T>(string uriRequest, StringContent content, CancellationToken cancellationToken = default)
        //{
        //    T response = default;
        //    NetworkAccess accessType = Connectivity.Current.NetworkAccess;

        //    if (accessType == NetworkAccess.Internet)
        //    {
        //        HttpResponseMessage httpResponse = await _client.PutAsync(uriRequest, content, cancellationToken);

        //        if (httpResponse.IsSuccessStatusCode)
        //        {
        //            var res = await httpResponse.Content.ReadAsStringAsync();
        //            response = JsonConvert.DeserializeObject<T>(res);
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception("Ooops, there is a problem with your internet connection");
        //    }

        //    return response;
        //}

        //public async Task<T> DeleteForResponseAsync<T>(string uriRequest, CancellationToken cancellationToken = default)
        //{
        //    T response = default;
        //    NetworkAccess accessType = Connectivity.Current.NetworkAccess;

        //    if (accessType == NetworkAccess.Internet)
        //    {
        //        HttpResponseMessage httpResponse = await _client.DeleteAsync(uriRequest, cancellationToken);

        //        if (httpResponse.IsSuccessStatusCode)
        //        {
        //            var res = await httpResponse.Content.ReadAsStringAsync();
        //            response = JsonConvert.DeserializeObject<T>(res);
        //        }
        //    }
        //    else
        //    {
        //        throw new Exception("Ooops, there is a problem with your internet connection");
        //    }

        //    return response;
        //}
    }
}
