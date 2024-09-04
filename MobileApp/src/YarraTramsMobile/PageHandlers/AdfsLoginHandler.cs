using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Core;
using Core.Helpers;
using Core.Analytics;
using Core.Models;
using Core.Security;
using Xamarin.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using YarraTramsMobile.Helpers;

namespace YarraTramsMobile.PageHandlers
{
    /// <summary>
    /// Active Directory Federated services login handler.  Orchestrates a login using javascript.
    /// </summary>
    public class AdfsLoginHandler : PageHandlerBase
    {
        protected internal string AdfsUrl;
        protected internal string TenancyId;
        private const string QueryStringRequirement = "ForceAuthentication=true&wauth=urn:oasis:names:tc:SAML:1.0:am:password&wfresh=0";
        private const string QueryStringCheck = "ForceAuthentication=true";
        private const string WinAuthStringCheck = "WIA";
        private int _redirectCount = 0;
        public AdfsLoginHandler(string tenancyId, string adfsUrl)
        {
            AdfsUrl = adfsUrl;
            TenancyId = tenancyId;
        }

        public override string Title => nameof(AdfsLoginHandler);

        public override bool CanHandle(string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url)) return false;
                if (string.IsNullOrEmpty(AdfsUrl)) return false;
                var loginBaseUrl = AdfsUrl.ToLower();
                var canHandle = !string.IsNullOrEmpty(url) && url.ToLower().StartsWith(loginBaseUrl) && !Utils.IsPayRollUrl(url);
                return canHandle;
            }
            catch (Exception ex)
            {
                AnalyticsService.ReportException(ex, $"Page handler ${this.Title} threw an exception on CanHandle");
                return false;
            }
        }

        public override async Task HandleNavigatingAsync(WebView webView, string url, UserCredential credential)
        {
            //Note: There is a redirect being hooked here at the Driod and iOS level.  See the JavascriptWebViewClient and the HybridWebViewRenderer
        }

        public override bool Redirect(string url, out string redirectUrl)
        {
            if (!string.IsNullOrEmpty(AdfsUrl) && url.ToLower().StartsWith(AdfsUrl.ToLower()) && !url.ToLower().Contains(QueryStringCheck.ToLower()))
            {
                //This is a customisation to prevent windows integrated.
                redirectUrl = url;

                // first check for Windows Integrated - this is getting set on the C-Connect network
                if (redirectUrl.Contains(WinAuthStringCheck, StringComparison.OrdinalIgnoreCase))
                {
                    //redirectUrl = redirectUrl.Replace(WinAuthStringCheck,string.Empty);
                    redirectUrl = Regex.Replace(redirectUrl, WinAuthStringCheck, string.Empty, RegexOptions.IgnoreCase);
                }

                if (redirectUrl.Contains("?"))
                {
                    redirectUrl += "&" + QueryStringRequirement;
                }
                else
                {
                    redirectUrl += "?" + QueryStringRequirement;
                }
                // log analytics message:
                string message = "Updated a URL from ADFS login handler";
                string last64Chars = redirectUrl.Substring(redirectUrl.Length - 64);
                var properties = new Dictionary<string, string> { { "RedirectURL", redirectUrl },{ "LastPartOfUrl",last64Chars }, { "Message", message } };
                AnalyticsService.ReportEvent(message, properties);
                return true;
            }
            redirectUrl = null;
            return false;
        }

        public override async Task HandleNavigatedAsync(WebView webView, string url, UserCredential credential)
        {
            HandleNavigated(webView, url, credential);
        }

        private void HandleNavigated(WebView webView, string url, UserCredential credential)
        {
            if (!url.ToLower().Contains(QueryStringCheck.ToLower()))
            {
                //This is a customisation to prevent windows integrated.
                return;
            }

            var usernameField = "UserName";
            var usernameValue = credential.Username;
            var passwordField = "Password";
            var passwordValue = credential.Password;

            //Create the settings object to hold config values and pass it in to the executeLogin method.  
            var settings = $"usernameField:'{usernameField}', usernameValue:'{usernameValue}', passwordField:'{passwordField}', passwordValue:'{passwordValue.ToInsecureString()}', onSubmit:Login.submitLoginRequest";
            var js = ScriptLoader.LoadLoginScript() + Environment.NewLine + "customPageHandler.executeLogin({" + settings + "});";
            Device.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    switch (Device.RuntimePlatform)
                    {
                        case Device.iOS:
                            (webView as HybridWebView).JavaScript = js;
                            break;
                        case Device.Android:
                            await webView.EvaluateJavaScriptAsync(js);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    AnalyticsService.ReportException(ex, $"Page handler ${this.Title} threw an exception on Navigated");
                }
            });
        }
    }
}