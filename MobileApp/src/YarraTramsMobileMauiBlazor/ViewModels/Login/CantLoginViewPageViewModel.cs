using CommunityToolkit.Mvvm.Messaging;
using Core;
using Core.Helpers;
using Core.Interfaces;
using MonkeyCache.FileStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTramsMobileMauiBlazor.Interfaces;
using YarraTramsMobileMauiBlazor.ViewModels.WebView;
using YarraTramsMobileMauiBlazor.Views.WebView;

namespace YarraTramsMobileMauiBlazor.ViewModels.Login
{
    public class CantLoginViewPageViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly IHttpUtility _httpUtility;
        private readonly IDatabaseService _databaseService;
        private readonly ICookieService _cookieService;
        private readonly IWebViewService _webViewService;

        private string? _userName;
        private string _helpDeskPhoneNumber = "03 9610 2707";


        public string? UserName
        {
            get { return _userName; }
            set { SetPropertyValue(ref _userName, value); }
        }


        public Command ResetPassword { get; set; }
        public Command CallHelpDesk { get; set; }
        public Command GoBack { get; set; }

        public CantLoginViewPageViewModel(
            INavigationService navigationService,
            IHttpUtility httpUtility,
            IDatabaseService databaseService,
            ICookieService cookieService,
            IWebViewService webViewService) : base(navigationService)
        {
            _navigationService = navigationService;
            _httpUtility = httpUtility;
            _databaseService = databaseService;
            _cookieService = cookieService;
            _webViewService = webViewService;
            
            ResetPassword = new Command(async () => await ResetPasswordCommand());
            CallHelpDesk = new Command(async () => await CallHelpDeskCommand());
            GoBack = new Command(GoBackCommand);
        }


        private async Task CallHelpDeskCommand()
        {
            try
            {
                await _webViewService.OpenExternalAppAsync($"tel:{_helpDeskPhoneNumber}");
            }
            catch (ArgumentNullException ex)
            {
                ShowAlert("Number was null or white space", $"{ex.Message}");

                await NavigateToLoginPage();
            }
            catch (FeatureNotSupportedException ex)
            {
                ShowAlert("Phone Dialer is not supported on this device.", $"{ex.Message}");

                await NavigateToLoginPage();
            }
            catch (Exception ex)
            {
                ShowAlert("Error", $"{ex.Message}");

                await NavigateToLoginPage();
            }
        }

        public async Task ResetPasswordCommand()
        {
            try
            {
                string resetPasswordUrl = Consts.PasswordResetUrl;
                //if (!string.IsNullOrEmpty(UserName))
                //{
                //    resetPasswordUrl = _httpUtility.ConcatUrls(Consts.PasswordResetUrl, string.Format("?username={0}", UserName));
                //}
                ResetUserDetails();

                Dictionary<string, object> _param = new Dictionary<string, object>();

                _param.Add("ResetPasswordUrl", resetPasswordUrl);

                await _navigationService.NavigateTo(nameof(SimpleWebViewPage));
                WeakReferenceMessenger.Default.Send<SimpleWebViewMessage>(new SimpleWebViewMessage(resetPasswordUrl));
            }
            catch (Exception ex)
            {
                ShowAlert("Error", $"{ex.Message}");

                await NavigateToLoginPage();
            }
        }

        private void ResetUserDetails()
        {
            AsyncHelper.RunSync(() => _databaseService.ClearTables());
            Barrel.Current.EmptyAll();
            _cookieService.ClearAllCookies();
        }

        public async Task NavigateToLoginPage()
        {
            await _navigationService.GoBack();
        }

        private void GoBackCommand(object obj)
        {
            _navigationService.GoBack();
        }
    }
}
