using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using YarraTramsMobileMauiBlazor.Interfaces;

namespace YarraTramsMobileMauiBlazor.Services
{
    public class HttpUtility : IHttpUtility
    {
        public string ConcatUrls(string firstPart, string secondPart)
        {
            // Simple URL concatenation handling slashes
            if (string.IsNullOrEmpty(firstPart)) return secondPart;
            if (string.IsNullOrEmpty(secondPart)) return firstPart;
            return firstPart.TrimEnd('/') + "/" + secondPart.TrimStart('/');
        }

        public async Task<string> GetResponseBody(string endpoint, string body, Dictionary<string, string> headers, string method, string session)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(new HttpMethod(method), endpoint);
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
            if (!string.IsNullOrEmpty(body))
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            throw new WebException($"Request failed with status code: {response.StatusCode}");
        }

        public async Task<Cookie?> GetResponseCookie(string endpoint, string body, Dictionary<string, string> headers, string method, string session, string cookieName)
        {
            var client = new HttpClient(new HttpClientHandler() { UseCookies = true, CookieContainer = new CookieContainer() });
            var request = new HttpRequestMessage(new HttpMethod(method), endpoint);
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }
            if (!string.IsNullOrEmpty(body))
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");
            }
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
                {
                    foreach (string cookie in cookies)
                    {
                        var parsedCookie = cookie.Split(';')[0].Split('=');
                        if (parsedCookie[0] == cookieName)
                        {
                            return new Cookie(parsedCookie[0], parsedCookie[1]);
                        }
                    }
                }
            }
            return null;
        }
    }
}
