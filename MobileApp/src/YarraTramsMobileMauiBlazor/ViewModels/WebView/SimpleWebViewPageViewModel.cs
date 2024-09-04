using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
using Core;
using YarraTramsMobileMauiBlazor.Views.Roster;
using YarraTramsMobileMauiBlazor.Views.YourSay;
using YarraTramsMobileMauiBlazor.Interfaces;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Core.Interfaces;

namespace YarraTramsMobileMauiBlazor.ViewModels.WebView
{
    public class SimpleWebViewPageViewModel : ViewModelBase
    {
        #region Fields
        private string _externalUrl = "";
        private IWebViewService _webViewService;
        private IConfigurationService _configService;

        #endregion Fields

        #region Properties
        public string ExternalUrl
        {
            get { return _externalUrl; }
            set { SetPropertyValue(ref _externalUrl, value); }
        }
        #endregion Properties

        #region Constructor

        public SimpleWebViewPageViewModel(INavigationService navigationService, IWebViewService webViewService, IConfigurationService configService) : base(navigationService)
        {
            _webViewService = webViewService;
            _configService = configService;
        }

        #endregion Constructor

        #region Methods

        public void OnAppearing()
        {
            // Register the messenger - to listen for when messages are sent from Mobile Web App to change navigation either to
            // native mobile OR another URL within the WebView page
            WeakReferenceMessenger.Default.Register<SimpleWebViewMessage>("App.ExternalNavigation", (r, message) =>
            {
                HandleNavigation(message.Value);
            });
        }

        public void OnDisappearing()
        {
            WeakReferenceMessenger.Default.Unregister<SimpleWebViewMessage>("App.ExternalNavigation");
        }


        public override Task OnNavigatingTo(Dictionary<string, object> parameter)
        {
            SetStatusBarColor();
            return base.OnNavigatingTo(parameter);
        }

        private void HandleNavigation(string url)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                ExternalUrl = url; // set the url of the webview to be the url that was tapped...
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

        #endregion Methods

    }

    public class SimpleWebViewMessage : ValueChangedMessage<string>
    {
        public SimpleWebViewMessage(string value) : base(value)
        {

        }
    }

}
