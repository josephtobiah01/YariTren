using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
using Core;
using YarraTramsMobileMauiBlazor.Views.Roster;
using YarraTramsMobileMauiBlazor.Views.YourSay;
using YarraTramsMobileMauiBlazor.Interfaces;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Core.Interfaces;
using YarraTramsMobileMauiBlazor.Views.WebView;
using YarraTramsMobileMauiBlazor.ViewModels.Roster;
using YarraTramsMobileMauiBlazor.Messages;
using System.Net;

namespace YarraTramsMobileMauiBlazor.ViewModels.WebView
{
    public class WebViewPageViewModel : ViewModelBase
    {
        #region Fields
        private string _webViewSource = "";
        private bool _isBusy;
        private bool _isDxPopupOpen;
        private IWebViewService _webViewService;
        private IConfigurationService _configService;
        private INavigationService _navigationService;
        private IServiceProvider _serviceProvider;

        #endregion Fields

        #region Properties

        public bool IsDxPopupOpen
        {
            get { return _isDxPopupOpen; }
            set { SetPropertyValue(ref _isDxPopupOpen, value); }
        }
        public string WebViewSource
        {
            get { return _webViewSource; }
            set { SetPropertyValue(ref _webViewSource, value); }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetPropertyValue(ref _isBusy, value); }
        }
        #endregion Properties

        #region Constructor

        public WebViewPageViewModel(
            INavigationService navigationService,
            IWebViewService webViewService, 
            IConfigurationService configService,
            IServiceProvider serviceProvider) : base(navigationService)
        {
            _navigationService = navigationService;
            _webViewService = webViewService;
            _configService = configService;
            _serviceProvider = serviceProvider;
            InitializeWebView();
#if ANDROID
            Microsoft.Maui.Handlers.WebViewHandler.Mapper.AppendToMapping("webview", (handler, View) =>
            {
                handler.PlatformView.Settings.AllowFileAccess = true;
                handler.PlatformView.Settings.AllowUniversalAccessFromFileURLs = true;
                handler.PlatformView.Settings.AllowFileAccessFromFileURLs = true;
            });

            WeakReferenceMessenger.Default.Register<PdfFileMessage>(this, (r, m) =>
            {
                string pdfViewSource = $"file:///android_asset/pdfjs/web/viewer.html?file=file:///android_asset/{WebUtility.UrlEncode(m.Value)}";
                WebViewSource = pdfViewSource;
            });

#endif
        }

#endregion Constructor

        #region Methods

        private void InitializeWebView()
        {
            try
            {
                IsBusy = true;
                // Register the messenger - to listen for when messages are sent from Mobile Web App to change navigation either to
                // native mobile OR another URL within the WebView page
                WeakReferenceMessenger.Default.Register<string>(App.Messages.PageNavigationMessage, (r, message) =>
                {
                    HandleNavigation(message);
                });

                // TODO: need to review - can we check to see if we have valid FedAuth and RTFA cookies?
                var url = Consts.AppUrl; //"https://jchmediagroup.sharepoint.com/sites/ytmobile-dev1/mobile/Pages/app.aspx#/news?web=1";
                WebViewSource = url; // new WebViewSource() { Url = url }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing WebView: {ex.Message}");
                WebViewSource = $"<h1>Error loading content: {ex.Message}</h1>";
                IsBusy = false;
            }
        }

        //public override Task OnNavigatingTo(Dictionary<string, object> parameter)
        //{
        //    IsBusy = true;
        //    IsDxPopupOpen = true;
        //    SetStatusBarColor();
        //    return base.OnNavigatingTo(parameter);
        //}

        //public override Task OnNavigatedTo()
        //{
        //    IsBusy = false;
        //    IsDxPopupOpen = false;
        //    return base.OnNavigatedTo();
            
        //}

        

        private void HandleNavigation(string url)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (url.Contains(Consts.AppUrl))
                {
                    //return; // navigation handled within the mobile web app...
                    WebViewSource = url;
                }
                else if (await _webViewService.ShouldOpenInDefaultApp(url, _configService))
                {
                    await _webViewService.OpenExternalAppAsync(url);
                    return;
                }
                else if (url.Contains("http"))
                {
                    await _navigationService.NavigateTo(nameof(SimpleWebViewPage));
                    WeakReferenceMessenger.Default.Send<SimpleWebViewMessage>(new SimpleWebViewMessage(url));
                }
                else
                {
                    if (url.Contains("roster"))
                    {
                        var rosterViewModel = _serviceProvider.GetRequiredService<RosterViewPageViewModel>();

                        // TODO:
                        //WeakReferenceMessenger.Default.Send<LoadingMessage>(new LoadingMessage(true));
                        //await Shell.Current.GoToAsync("//RosterViewPage", true);
                        //WeakReferenceMessenger.Default.Send<LoadingMessage>(new LoadingMessage(false));


                        // TODO: Roster takes a few seconds to load, can we put in a loader/spinner while it loads?
                        await Shell.Current.GoToAsync(nameof(RosterViewPage), true);
                    }
                    else if (url.Contains("yoursay"))
                    {
                        await Shell.Current.GoToAsync("YourSayViewPage", true);
                        //await _navigationService.NavigateTo(nameof(YourSayViewPage));
                    }
                    IsBusy = false;
                }
            });
        }

        private void SetStatusBarColor()
        {
            var statusBarBehavior = Shell.Current?.CurrentPage?.Behaviors?.OfType<StatusBarBehavior>().FirstOrDefault();
            if (statusBarBehavior != null)
            {
                statusBarBehavior.StatusBarColor = Colors.Gray;
                statusBarBehavior.StatusBarStyle = StatusBarStyle.DarkContent;
            }
        }

        public void RestoreWebViewState()
        {
            if (string.IsNullOrEmpty(WebViewSource))
            {
                WebViewSource = Consts.AppUrl;
            }
        }

        public void ResetState()
        {
            // Reset the ViewModel's properties
            WebViewSource = Consts.AppUrl;
            IsBusy = false;
            IsDxPopupOpen = false;
        }

        #endregion Methods

    }

    public class SendItemMessage : ValueChangedMessage<string>
    {
        public SendItemMessage(string value) : base(value)
        {
        }
    }
}
