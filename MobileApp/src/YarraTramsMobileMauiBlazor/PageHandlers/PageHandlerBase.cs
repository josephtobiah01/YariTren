using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTramsMobileMauiBlazor.PageHandlers
{
    public abstract class PageHandlerBase : IPageHandler
    {
        public abstract string Title { get; }
        public abstract bool CanHandle(string url);
        public abstract Task HandleNavigatingAsync(WebView webView, string url, UserCredential credential);
        public abstract Task HandleNavigatedAsync(WebView webView, string url, UserCredential credential);
        public virtual bool Redirect(string url, out string redirectUrl)
        {
            redirectUrl = null;
            return false;
        }
        public virtual string LoadingText { get; set; } = "Logging in, please wait...";

    }
}
