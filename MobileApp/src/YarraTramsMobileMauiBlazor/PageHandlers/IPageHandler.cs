using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTramsMobileMauiBlazor.PageHandlers
{
    public interface IPageHandler
    {
        string Title { get; }
        bool CanHandle(string url);
        Task HandleNavigatingAsync(WebView webView, string url, UserCredential credential);
        Task HandleNavigatedAsync(WebView webView, string url, UserCredential credential);
        bool Redirect(string url, out string redirectUrl);

    }
}
