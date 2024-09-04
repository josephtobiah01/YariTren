using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Core.Helpers
{
    public class HttpUtility
    {
        public static string GetResponseBody(string endpoint, string body, Dictionary<string, string> headers, string method, string session, CookieContainer cookies)
        {
            HttpWebRequest endpointRequest = CreateRequest(endpoint, body, headers, method, session);
            if (cookies != null)
            {
                endpointRequest.CookieContainer = cookies;
            }
            try
            {
                using (HttpWebResponse endpointResponse = (HttpWebResponse)endpointRequest.GetResponse())
                {
                    return GetResponseBodyAsString(endpointResponse);
                }
            }
            catch (WebException) // catch all - to handle invalid requests, 404's etc.  This can be updated in the future to better handle non-200 responses
            {
                throw;
            }

        }

        public static Cookie GetResponseCookie(string endpoint, string body, Dictionary<string, string> headers, string method, string session, string cookieName)
        {
            HttpWebRequest endpointRequest = CreateRequest(endpoint, body, headers, method, session);
            endpointRequest.CookieContainer = new CookieContainer();
            try
            {
                using (HttpWebResponse endpointResponse = (HttpWebResponse)endpointRequest.GetResponse())
                {
                    return GetCookieFromResponse(endpointResponse, cookieName);
                }
            }
            catch (WebException) // catch all - to handle invalid requests, 404's etc.  This can be updated in the future to better handle non-200 responses
            {
                throw;
            }
        }

        internal static HttpWebRequest CreateRequest(string endpoint, string body, Dictionary<string, string> headers, string method, string session)
        {
            Stream requestStream = null;
            HttpWebRequest endpointRequest = (HttpWebRequest)WebRequest.Create(endpoint);
            endpointRequest.Method = method ?? "GET";

            // Add headers
            if (headers != null)
            {
                foreach (KeyValuePair<string, string> keyVal in headers)
                {
                    if (keyVal.Key == "Content-Type")
                    {
                        endpointRequest.ContentType = keyVal.Value;
                        continue;
                    }
                    endpointRequest.Headers.Add(keyVal.Key, keyVal.Value);
                }
            }

            if (!string.IsNullOrEmpty(body))
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(body);
                requestStream = endpointRequest.GetRequestStream();
                requestStream.Write(byteArray, 0, byteArray.Length);
                requestStream.Close();
            }
            return endpointRequest;
        }

        internal static string GetResponseBodyAsString(HttpWebResponse response)
        {
            Encoding enc = Encoding.GetEncoding(response.CharacterSet);
            string responseString;
            using (StreamReader loResponseStream = new StreamReader(response.GetResponseStream(), enc))
            {
                responseString = loResponseStream.ReadToEnd();
            }
            response.Close();
            return responseString;
        }

        internal static Cookie GetCookieFromResponse(HttpWebResponse response, string cookieName)
        {
            try
            {
                return response.Cookies[cookieName] ?? null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        ///   From the SP2010 API - takes two parts of URL and joins them together.
        /// </summary>
        /// <param name = "firstPart"></param>
        /// <param name = "secondPart"></param>
        /// <returns></returns>
        public static string ConcatUrls(string firstPart, string secondPart)
        {
            if (firstPart == null)
            {
                return secondPart;
            }
            if (secondPart == null)
            {
                return secondPart;
            }
            string str = "/";
            if (firstPart.EndsWith(str, StringComparison.OrdinalIgnoreCase))
            {
                if (secondPart.StartsWith(str, StringComparison.OrdinalIgnoreCase))
                {
                    firstPart = firstPart.TrimEnd(str.ToCharArray());
                }
                return (firstPart + secondPart);
            }
            if (secondPart.StartsWith(str, StringComparison.OrdinalIgnoreCase))
            {
                return (firstPart + secondPart);
            }
            return (firstPart + str + secondPart);
        }

        public static string GetServerRelUrlFromFullUrl(string url)
        {
            int index = url.IndexOf("//");
            if ((index < 0) || (index == (url.Length - 2)))
            {
                throw new ArgumentException();
            }
            int startIndex = url.IndexOf('/', index + 2);
            if (startIndex < 0)
            {
                return "/";
            }
            string str = url.Substring(startIndex);
            if (str.IndexOf("?") >= 0)
                str = str.Substring(0, str.IndexOf("?"));
            if (str.IndexOf(".aspx") > 0)
                str = str.Substring(0, str.LastIndexOf("/"));
            if ((str.Length > 1) && (str[str.Length - 1] == '/'))
            {
                return str.Substring(0, str.Length - 1);
            }
            return str;
        }

        public static string GetHostFromFullUrl(string url)
        {
            var uri = new Uri(url);
            //return string.Format("{0}://{1}", uri.Scheme, uri.Host);
            return $"{uri.Scheme}://{uri.Host}";
        }

        public static string GetSPSitePathFromFullUrl(string url)
        {
            var uri = new Uri(url);
            var path = uri.PathAndQuery;
            var pathParts = path.Split('/');
            if (pathParts.Count() > 2)
            {
                return $"{uri.Scheme}://{uri.Host}/{pathParts[1]}/{pathParts[2]}";
            }
            return GetHostFromFullUrl(url);
        }


        public static bool IsFullUrl(string url)
        {
            return !string.IsNullOrEmpty(url) && url.IndexOf("//") > 0;
        }

        public static string GetFileNameFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return string.Empty;
            }

            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                uri = new Uri(null, url);

            string file = Path.GetFileName(uri.LocalPath);
            return file;
        }

        public static bool IsPDFUrl(string url)
        {
            string fileName = GetFileNameFromUrl(url);
            bool isPDF = url.StartsWith("http") && (url.EndsWith("pdf") || fileName.EndsWith("pdf"));
            return isPDF;
        }
    }
}
