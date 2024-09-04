using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IHttpUtility
    {
        string ConcatUrls(string firstPart, string secondPart);
        Task<string> GetResponseBody(string endpoint, string body, Dictionary<string, string> headers, string method, string session);
        Task<Cookie> GetResponseCookie(string endpoint, string body, Dictionary<string, string> headers, string method, string session, string cookieName);
    }
}
