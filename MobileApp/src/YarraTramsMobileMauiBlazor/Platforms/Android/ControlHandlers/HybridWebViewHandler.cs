using Android.Webkit;
using Core.Interfaces;
using Microsoft.Maui.Handlers;
using YarraTramsMobileMauiBlazor.CustomHandlers;
using YarraTramsMobileMauiBlazor.Interfaces;
using YarraTramsMobileMauiBlazor.Services;
using WebView = Android.Webkit.WebView;
using static Android.Views.ViewGroup;
using Microsoft.Maui.Platform;


namespace YarraTramsMobileMauiBlazor.Platforms.Android.ControlHandlers
{
    public partial class HybridWebViewHandler : ViewHandler<IHybridWebView, WebView>
    {
        public static PropertyMapper<IHybridWebView, HybridWebViewHandler> HybridWebViewMapper = new PropertyMapper<IHybridWebView, HybridWebViewHandler>(ViewHandler.ViewMapper);
        const string JavaScriptFunction = "function invokeNativeLink(data){jsBridge.invokeAction(data);} function invokeAnalytics(data){jsBridge.invokeAnalytics(data);} function invokeCheckPassword(data){jsBridge.invokeCheckPassword(data);}";

        private JSBridge _jsBridgeHandler;
        private IWebViewService? _webViewService => ServiceLocator.Current?.GetService<IWebViewService>();
        private IAnalyticsService? _analyticsService => ServiceLocator.Current?.GetService<IAnalyticsService>();

        public HybridWebViewHandler() : base(HybridWebViewMapper)
        {
            _jsBridgeHandler = new JSBridge(this, _analyticsService);
        }

        protected override WebView CreatePlatformView()
        {
            var webView = new WebView(Context)
            {
                LayoutParameters = new LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent)
            };
            
            webView.Settings.JavaScriptEnabled = true;
            webView.Settings.DomStorageEnabled = true;
            webView.Settings.AllowContentAccess = true;
            webView.Settings.AllowFileAccess = true;

            webView.AddJavascriptInterface(_jsBridgeHandler, "jsBridge");
            webView.SetWebViewClient(new JavascriptWebViewClient($"javascript: {JavaScriptFunction}", (HybridWebView)VirtualView, _analyticsService));
            webView.SetWebChromeClient(new WebChromeClient());
            return webView;
        }

        protected override void ConnectHandler(WebView platformView)
        {
            base.ConnectHandler(platformView);
            if (VirtualView.Source != null)
            {
                LoadSource(VirtualView.Source, PlatformView);
            }
            VirtualView.SourceChanged += VirtualView_SourceChanged;
            //VirtualView.RequestEvaluateJavaScript += VirtualView_RequestEvaluateJavaScript;
        }

        protected override void DisconnectHandler(WebView platformView)
        {
            VirtualView.SourceChanged -= VirtualView_SourceChanged;
            VirtualView.Cleanup();
            _jsBridgeHandler?.Dispose();
            base.DisconnectHandler(platformView);
        }

        private void VirtualView_SourceChanged(object? sender, SourceChangedEventArgs e)
        {
            LoadSource(e.Source, PlatformView);
        }

        private static void LoadSource(WebViewSource source, WebView control)
        {
            try
            {
                if (source is HtmlWebViewSource html)
                {
                    control.LoadDataWithBaseURL(html.BaseUrl, html.Html, null, "charset=UTF-8", null);
                }
                else if (source is UrlWebViewSource url)
                {
                    control.LoadUrl(url.Url);
                }
            }
            catch { }
        }

       // private void VirtualView_RequestEvaluateJavaScript
            

    }

}
