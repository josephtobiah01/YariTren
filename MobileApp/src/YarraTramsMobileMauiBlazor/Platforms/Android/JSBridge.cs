using Android.Webkit;
using Core.Analytics;
using Core.Interfaces;
using Core.Security;
using Java.Interop;
using YarraTramsMobileMauiBlazor.CustomHandlers;
using YarraTramsMobileMauiBlazor.Interfaces;
using YarraTramsMobileMauiBlazor.Platforms.Android.ControlHandlers;
using YarraTramsMobileMauiBlazor.Services;

namespace YarraTramsMobileMauiBlazor.Platforms.Android
{
    public class JSBridge : Java.Lang.Object
    {
        readonly WeakReference<HybridWebViewHandler> hybridWebViewHandler;
        private readonly IAnalyticsService _analyticsService;
        IUtilitiesService? _utilitiesService => ServiceLocator.Current?.GetService<IUtilitiesService>();

        public JSBridge(
            HybridWebViewHandler handler,
            IAnalyticsService analyticsService
            )
        {
            hybridWebViewHandler = new WeakReference<HybridWebViewHandler>(handler);
            _analyticsService = analyticsService;
           
        }

        [JavascriptInterface]
        [Export("invokeAction")]
        public async void InvokeAction(string data)
        {
            try
            {
                HybridWebViewHandler handler;
                if (hybridWebViewHandler != null && hybridWebViewHandler.TryGetTarget(out handler))
                {
                    ((HybridWebView)handler.VirtualView).InvokeAction(data);
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage?.DisplayAlert("Error", $"{ex.Message}", "Ok");
            }
        }

        [JavascriptInterface]
        [Export("invokeAnalytics")]
        public async void InvokeAnalytics(string data)
        {
            try
            {
                _utilitiesService.TrackUserAction(data);
            }
            catch (Exception ex)
            {
                //MainPage? mainPage = new MainPage();
                //Utils.HandleError(ex);
                await Application.Current.MainPage.DisplayAlert("Error", $"{ex.Message}", "Ok");
            }
        }

        [JavascriptInterface]
        [Export("invokeCheckPassword")]
        public async void InvokeCheckPasswordAsync(string data)
        {
            try
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var securePassword = data.ToSecureString();
                    data = null;
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage?.DisplayAlert("Error", $"{ex.Message}", "Ok");
            }
        }
        
    }
}
