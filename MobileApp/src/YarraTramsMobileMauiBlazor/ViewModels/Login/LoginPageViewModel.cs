using Core;
using Core.Analytics;
using Core.Exceptions;
using Core.Helpers;
using Core.Interfaces;
using Data.SharePoint.Authentication;
using DevExpress.Maui.Controls;
using MonkeyCache.FileStore;
using YarraTramsMobileMauiBlazor.Views.Registration;
using YarraTramsMobileMauiBlazor.Interfaces;
using YarraTramsMobileMauiBlazor.Views.Login;

namespace YarraTramsMobileMauiBlazor.ViewModels.Login
{
    public class LoginPageViewModel : ViewModelBase
    {
        #region Fields

        private readonly IDatabaseService _databaseService;
        private readonly ICookieService _cookieService;
        private readonly IPinManager _pinManager;
        private readonly IRosterService _rosterService;
        private readonly IUserProfileService _userProfileService;
        private readonly IMobileDeviceService _mobileDeviceService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IAnalyticsService _analyticsService;

        private DXPopup _dxPopup;
        private bool _isSignedIn;
        private bool _signInBtnVisible = true;
        private bool _isSigningIn;
        private bool _isEnabled;
        private string? _name;
        private string _accessToken;

        #endregion Fields

        #region Properties

        public string AccessToken
        {
            get { return _accessToken; }
            set { SetPropertyValue(ref _accessToken, value); }
        }
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetPropertyValue(ref _isEnabled, value); }
        }

        public bool IsSignedIn
        {
            get { return _isSignedIn; }
            set { SetPropertyValue(ref _isSignedIn, value); }
        }

        public bool SignInBtnVisible
        {
            get { return _signInBtnVisible; }
            set { SetPropertyValue(ref _signInBtnVisible, value); }
        }

        public bool IsSigningIn
        {
            get { return _isSigningIn; }
            set { SetPropertyValue(ref _isSigningIn, value); }
        }

        public string? Name
        {
            get { return _name; }
            set { SetPropertyValue(ref _name, value); }
        }

        public DXPopup DXPopup
        {
            get { return _dxPopup; }
            set { SetPropertyValue(ref _dxPopup, value); }
        }


        public Command SignIn { get; set; }
        public Command ForgotLogin { get; set; }
        public Command Register { get; set; }
        //public Command TermsOfUse { get; set; }


        #endregion Properties

        #region Constructor

        public LoginPageViewModel(
            INavigationService navigationService,
            IDatabaseService databaseService,
            ICookieService cookieService,
            IPinManager pinManager,
            IRosterService rosterService,
            IUserProfileService userProfileService,
            IMobileDeviceService mobileDeviceService,
            IServiceProvider serviceProvider,
            IAnalyticsService analyticsService) : base(navigationService)
        {

            _databaseService = databaseService;
            _cookieService = cookieService;
            _pinManager = pinManager;
            _rosterService = rosterService;
            _userProfileService = userProfileService;
            _mobileDeviceService = mobileDeviceService;
            _serviceProvider = serviceProvider;
            _analyticsService = analyticsService;

            IsEnabled = true;

            SignIn = new Command(SignInCommand);
            ForgotLogin = new Command(ForgotLoginCommand);
            Register = new Command(RegisterCommand);
            //TermsOfUse = new Command(TermsOfUseCommand);

            InitializeDxPopup();
            _serviceProvider = serviceProvider;
        }

        #endregion Constructor

        #region Methods

        private void Initialise()
        {

        }

        private void ResetUserDetails()
        {
            AsyncHelper.RunSync(() => _databaseService.ClearTables());
            Barrel.Current.EmptyAll();
            _cookieService.ClearAllCookies();
        }

        private void ShowDxPopup()
        {
            if (DXPopup != null)
            {
                DXPopup.IsOpen = true;
            }
        }

        private void HideDxPopup()
        {
            if (DXPopup != null)
            {
                DXPopup.IsOpen = false;
            }
        }


        private void InitializeDxPopup()
        {
            DXPopup = new DXPopup
            {
                HeightRequest = 200,
                WidthRequest = 300,
                IsOpen = false, // Ensure the popup starts closed
                Content = new Label
                {
                    Text = "Loading...",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                }
            };
        }

        private async void SignInCommand(object obj)
        {
            try
            {
                IsSigningIn = true;
                bool loginSuccess = false;
                ResetUserDetails();
                await SPAuthenticator.Instance.ClearTokenCacheAsync();
                ShowDxPopup();
                loginSuccess = await SPAuthenticator.Instance.Login();
                if (loginSuccess)
                {
                    IsEnabled = false;
                    var userProfile = await _userProfileService.GetCurrentUserProfile();
                    if (_rosterService.HasRoster(userProfile))
                    {
                        await _rosterService.Load(userProfile, true);
                    }
                    _signInBtnVisible = false;
                    _mobileDeviceService.RunBackgroundTasks(userProfile);

                    //Dictionary<string, object> _param = new Dictionary<string, object>();
                    //_param.Add("InitialViewState", AuthenticateViewState.AddPin);
                    //await _navigationService.NavigateTo(nameof(PinPage), _param);
                    //await Shell.Current.GoToAsync("//PinPage");
                    var pinPage = _serviceProvider.GetRequiredService<PinPage>();
                    Application.Current.MainPage = pinPage;
                }
                else
                {
                    HideDxPopup();
                    ShowAlert("Error", "Login failed, please check your username and password.");
                }
            }
            catch (InvalidUsernameOrPasswordException)
            {
                HideDxPopup();
                ShowAlert("Invalid username or password", $"{Consts.InvalidUserNameOrPasswordMessage}");
            }
            catch (AccountDisabledException)
            {
                HideDxPopup();
                ShowAlert("Account locked", $"{Consts.LockedAccountMessage}");
            }
            catch (SystemUnavailableException ex)
            {
                IsEnabled = true;
                ShowAlert("Connection Error", "The system is currently un-available or the connection to the Internet is not working.");
            }
        }

        private void ForgotLoginCommand(object obj)
        {
            NavigationService.NavigateTo(nameof(CantLoginViewPage));
        }

        public void RegisterCommand(object obj)
        {
            try
            {
                //var page = _serviceProvider.GetRequiredService<RegistrationPage>();
                //Application.Current.MainPage = page;
                NavigationService.NavigateTo(nameof(RegistrationPage));
            }
            catch (Exception ex)
            {
                ShowAlertAsync("Unexpected error", "An unexpected error occurred, please try again later.");
                _analyticsService.ReportException(ex);
            }
        }

        public Command TermsOfUse
        {
            get
            {
                return new Command(async () =>
                {
                    try
                    {
                        await ShowAlertAsync("Terms of Use", "This system (including all related equipment, networks and network devices) is the property of KDR Victoria Pty Ltd(Yarra Trams) and is for authorised access and use only.By proceeding to access and use this system, you acknowledge and agree to the below" + Environment.NewLine + Environment.NewLine +
                                "1 - You are authorised by Yarra Trams to access and use this system;" + Environment.NewLine +
                                "2 - You are using your own unique login identification provided by Yarra Trams; " + Environment.NewLine +
                                "3 - You agree to all applicable Yarra Trams policies including security and acceptable use policies; and" + Environment.NewLine +
                                "4 - All activity, which includes your activity, on this system is monitored and audited for lawful and unlawful activities, and no privacy is guaranteed by Yarra Trams, except in accordance with applicable privacy law." + Environment.NewLine + Environment.NewLine +
                                "You must contact Yarra Trams Service Desk(+61 3 9610 2707) if you require clarification or further information on the access and use of this system.");
                    }
                    catch (Exception ex)
                    {
                        await ShowAlertAsync("Unexpected error", "An unexpected error occurred, please try again later.");
                        _analyticsService.ReportException(ex);
                    }
                });
            }
        }


        #endregion Methods

    }
}
