using Core;
using Core.Analytics;
using Core.Interfaces;
using Core.Models;
using CoreGraphics;
using Data.SharePoint.Clients;
using Foundation;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using ObjCRuntime;
using System.Security;
using WebKit;
using YarraTramsMobileMauiBlazor.CustomHandlers;
using YarraTramsMobileMauiBlazor.Interfaces;
using YarraTramsMobileMauiBlazor.PageHandlers;
using YarraTramsMobileMauiBlazor.Services;
using HttpUtility = Core.Helpers.HttpUtility;

namespace YarraTramsMobileMauiBlazor.Platforms.iOS.ControlHandlers
{
    public class HybridWebViewHandler : ViewHandler<IHybridWebView, WKWebView>
    {
        public static PropertyMapper<IHybridWebView, HybridWebViewHandler> HybridWebViewMapper = new PropertyMapper<IHybridWebView, HybridWebViewHandler>(ViewHandler.ViewMapper);
        IAnalyticsService _analyticsService => ServiceLocator.GetService<IAnalyticsService>();
        
        const string JavaScriptFunction = "function invokeNativeLink(data){window.webkit.messageHandlers.invokeAction.postMessage(data);} function invokeAnalytics(data){var message = 'Analytics:' + data; window.webkit.messageHandlers.invokeAnalytics.postMessage(message);} function invokeCheckPassword(data){var message = 'ChangePassword:' + data; window.webkit.messageHandlers.invokeCheckPassword.postMessage(message);}";
        private WKUserContentController userController;
        private JSBridge jsBridgeHandler;
        private IOSNavigationDelegate navigationDelegate;
        static SynchronizationContext sync;

        public HybridWebViewHandler() : base(HybridWebViewMapper)
        {
            sync = SynchronizationContext.Current;
            jsBridgeHandler = new JSBridge(this, _analyticsService);
            navigationDelegate = new IOSNavigationDelegate(this);
            userController = new WKUserContentController();
        }

        public NativeHandle Handle => throw new NotImplementedException();

        //private AdfsLoginHandler _loginHandler = new AdfsLoginHandler(Consts.TenancyId, Consts.CustomSignInPage);

        protected override WKWebView CreatePlatformView()
        {
            sync = sync ?? SynchronizationContext.Current;
            
            var script = new WKUserScript(new NSString(JavaScriptFunction), WKUserScriptInjectionTime.AtDocumentEnd, false);
            userController.AddUserScript(script);
            userController.AddScriptMessageHandler(jsBridgeHandler, "invokeAction");
            userController.AddScriptMessageHandler(jsBridgeHandler, "invokeAnalytics");
            userController.AddScriptMessageHandler(jsBridgeHandler, "invokeCheckPassword");
            var config = new WKWebViewConfiguration { UserContentController = userController };
            var webView = new WKWebView(CGRect.Empty, config);
            webView.NavigationDelegate = navigationDelegate;
            //webView.AddObserver(new Foundation.NSString("URL"), Foundation.NSKeyValueObservingOptions.OldNew, (o) =>
            //{
            //    Console.WriteLine(o.ToString());
            //});

            return webView;
        }

        protected override void ConnectHandler(WKWebView platformView)
        {
            base.ConnectHandler(platformView);
            //platformView.NavigationDelegate = jsBridgeHandler;

            if (VirtualView.Source != null)
            {
                LoadSource(VirtualView.Source, PlatformView);
            }
            VirtualView.SourceChanged += VirtualView_SourceChanged;
        }

        protected override void DisconnectHandler(WKWebView platformView)
        {
            base.DisconnectHandler(platformView);
            VirtualView.SourceChanged -= VirtualView_SourceChanged;
            userController.RemoveAllUserScripts();
            userController.RemoveScriptMessageHandler("invokeAction");
            userController.RemoveScriptMessageHandler("invokeAnalytics");
            userController.RemoveScriptMessageHandler("invokeCheckPassword");
            jsBridgeHandler?.Dispose();
            platformView.NavigationDelegate = null;
        }

        private void VirtualView_RequestEvaluateJavaScript(object sender, EvaluateJavaScriptAsyncRequest e)
        {
            sync.Post((o) =>
            {
                PlatformView.EvaluateJavaScript(e);
            }, null);
        }

        private void VirtualView_SourceChanged(object? sender, SourceChangedEventArgs e)
        {
            LoadSource(e.Source, PlatformView);
        }

        private static void LoadSource(WebViewSource source, WKWebView control)
        {
            if (source is HtmlWebViewSource html)
            {
                control.LoadHtmlString(html.Html, new NSUrl(html.BaseUrl ?? "http://localhost", true));
            }
            else if (source is UrlWebViewSource url)
            {
                control.LoadRequest(new NSUrlRequest(new NSUrl(url.Url)));
            }
        }



    }

    public class JSBridge : NSObject, IWKScriptMessageHandler
    {
        IAnalyticsService _analyticsService;
        IUserProfileService _userProfileService => ServiceLocator.GetService<IUserProfileService>();
        IUtilitiesService? _utilitiesService => ServiceLocator.Current?.GetService<IUtilitiesService>();

        readonly WeakReference<HybridWebViewHandler> hybridWebViewRenderer;
        internal JSBridge(HybridWebViewHandler hybridRenderer, IAnalyticsService analyticsService)
        {
            hybridWebViewRenderer = new WeakReference<HybridWebViewHandler>(hybridRenderer);
            _analyticsService = analyticsService;
        }

        public void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
        {
            // Handle the received script message
            var messageBody = message.Body.ToString();
            if (messageBody.StartsWith("Analytics:"))
            {
                // Handle Analytics message
                _utilitiesService.TrackUserAction(messageBody.Replace("Analytics:", string.Empty));
            }
            //else if (messageBody.StartsWith("ChangePassword"))
            //{
            //    // Handle ChangePassword message
            //    var password = messageBody.Replace("ChangePassword:", string.Empty);
            //    if (!string.IsNullOrEmpty(password))
            //    {
            //        var securePassword = ToSecureString(password);
            //        //PageHandlers.PageHandlerAsync._pword = securePassword;
            //    }
            //}
            else
            {
                HybridWebViewHandler hybridRenderer;
                if (hybridWebViewRenderer.TryGetTarget(out hybridRenderer))
                {
                    hybridRenderer.VirtualView?.InvokeAction(message.Body.ToString());
                }
            }
        }
    }

    public class IOSNavigationDelegate : WKNavigationDelegate
    {
        readonly WeakReference<HybridWebViewHandler> hybridWebViewRenderer;
        AdfsLoginHandler _loginHandler = new AdfsLoginHandler(Consts.TenancyId, Consts.CustomSignInPage);
        IAnalyticsService _analyticsService => ServiceLocator.GetService<IAnalyticsService>();
        public IOSNavigationDelegate(HybridWebViewHandler hybridRenderer)
        {
            hybridWebViewRenderer = new WeakReference<HybridWebViewHandler>(hybridRenderer);
        }

        //[Export("webView:didFinishNavigation:")]
        public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            Console.WriteLine("here");
        }

       // [Export("webView:didStartProvisionalNavigation:")]
        public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
        {
            var url = webView.Url?.AbsoluteString;
            if (HttpUtility.IsPDFUrl(url))
            {
                webView.StopLoading();
                HybridWebViewHandler hybridRenderer;
                if (hybridWebViewRenderer.TryGetTarget(out hybridRenderer))
                {
                    hybridRenderer.VirtualView?.InvokeAction(url);
                }
            }
            if (!string.IsNullOrEmpty(url) && _loginHandler.CanHandle(url) && _loginHandler.Redirect(url, out var redirectUrl))
            {
                webView.StopLoading();
                // Log analytics message:
                _analyticsService.ReportEvent("Stopped loading a particular URL", new Dictionary<string, string> { { "URL", url }, { "LastPartOfUrl", url.Substring(url.Length - 64) } });

                webView.LoadRequest(new NSUrlRequest(new NSUrl(redirectUrl)));
                // Log analytics message:
                _analyticsService.ReportEvent("Redirected to a different URL", new Dictionary<string, string> { { "RedirectURL", redirectUrl }, { "LastPartOfUrl", redirectUrl.Substring(redirectUrl.Length - 64) } });

                return;
            }
            //webView?.InvokeNavigatingAction(url);

        }




    }

}
