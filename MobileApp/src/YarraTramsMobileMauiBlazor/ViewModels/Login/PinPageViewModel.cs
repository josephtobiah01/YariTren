using Core;
using Core.Exceptions;
using Core.Helpers;
using Core.Interfaces;
using Core.Models;
using Core.Security;
using Data.SharePoint.Authentication;
using DevExpress.Maui.Controls;
using MonkeyCache.FileStore;
using YarraTramsMobileMauiBlazor.Interfaces;
using YarraTramsMobileMauiBlazor.Views.Login;

namespace YarraTramsMobileMauiBlazor.ViewModels.Login
{

    public enum AuthenticateViewState { AddPin, PinAuth, ConfirmPin, Congrats };

    //[QueryProperty(nameof(InitialViewStateData), "InitialViewStateData")]
    public class PinPageViewModel : ViewModelBase
    {
        #region Fields
        //bool
        private bool _isValid;
        private bool _titleVisible;
        private bool _isEnabled;
        private bool _forgetPin;
        private bool _isBusy;

        //string
        private string? _previousPin;
        private string? _pin;
        private string? _signInText;
        private string? _title;
        private string? _pinInstructionalText;
        private string? _placeholderText;

        //int
        private int _pinAttempts = 0;
        private int _lblPinHelpTextRow = 3;

        //reference
        private readonly INavigationService _navigationService;
        private readonly IDatabaseService _databaseService;
        private readonly IPinManager _pinManager;
        private readonly IAnalyticsService _analyticsService;
        private readonly ICookieService _cookieService;
        private readonly IRosterService _rosterService;
        private readonly IUserProfileService _userProfileService;
        private readonly IMobileDeviceService _mobileDeviceService;
        private readonly IServiceProvider _serviceProvider;
        private AuthenticateViewState _state;
        private AuthenticateViewState _initialViewStateData;
        private AuthenticatedUser? _localUser;
        

        #endregion Fields

        #region Properties

        //BOOL

        public bool IsBusy
        {
            get { return _isBusy; }
            set { SetPropertyValue(ref _isBusy, value); }
        }
        public bool TitleVisible
        {
            get { return _titleVisible; }
            set { SetPropertyValue(ref _titleVisible, value); }
        }
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set { SetPropertyValue(ref _isEnabled, value); }
        }
        public bool IsValid
        {
            get { return _isValid; }
            set { SetPropertyValue(ref _isValid, value); }
        }
        public bool ForgetPin
        {
            get { return _forgetPin; }
            set { SetPropertyValue(ref _forgetPin, value); }
        }
        //STRING

        public string? PlaceholderText
        {
            get { return _placeholderText; }
            set { SetPropertyValue(ref _placeholderText, value); }
        }
        public string? SignInText
        {
            get { return _signInText; }
            set { SetPropertyValue(ref _signInText, value); }
        }
        public string? Title
        {
            get { return _title; }
            set { SetPropertyValue(ref _title, value); }
        }
        public string? PreviousPin
        {
            get { return _previousPin; }
            set { SetPropertyValue(ref _previousPin, value); }
        }
        public string? Pin
        {
            get { return _pin; }
            set { SetPropertyValue(ref _pin, value); }
        }
        public string? PinInstructionalText
        {
            get { return _pinInstructionalText; }
            set { SetPropertyValue(ref _pinInstructionalText, value); }
        }
        //INT

        public int PinAttempts
        {
            get { return _pinAttempts; }
            set { SetPropertyValue(ref _pinAttempts, value); }
        }
        public int LblPinHelpTextRow
        {
            get { return _lblPinHelpTextRow; }
            set { SetPropertyValue(ref _lblPinHelpTextRow, value); }
        }

        //REFERENCE

        public AuthenticatedUser? LocalUser
        {
            get { return _localUser; }
            set { SetPropertyValue(ref _localUser, value); }
        }
        public AuthenticateViewState State
        {
            get { return _state; }
            set { SetPropertyValue(ref _state, value); }
        }
        public AuthenticateViewState InitialViewStateData
        {
            get { return _initialViewStateData; }
            set { SetPropertyValue(ref _initialViewStateData, value); }
        }

        
        public Command ForgotPin { get; set; }
        public Command ExecuteSignin { get; set; }


        #endregion Properties

        #region Constructor

        public PinPageViewModel(
            INavigationService navigationService,
            IDatabaseService databaseService,
            IPinManager pinManager,
            IAnalyticsService analyticsService,
            ICookieService cookieService,
            IRosterService rosterService,
            IUserProfileService userProfileService,
            IMobileDeviceService mobileDeviceService,
            IServiceProvider serviceProvider) : base(navigationService)
        {
            _navigationService = navigationService;
            _databaseService = databaseService;
            _pinManager = pinManager;
            _analyticsService = analyticsService;
            _cookieService = cookieService;
            _rosterService = rosterService;
            _userProfileService = userProfileService;
            _mobileDeviceService = mobileDeviceService;
            _serviceProvider = serviceProvider;

            Initialize();
            

            
            ForgotPin = new Command(ForgotPinCommand);

            ExecuteSignin = new Command(ExecuteSigninCommand);

        }

        


        

        #endregion Constructor

        #region Methods

        public void Initialize()
        {
            Pin = string.Empty;
            SignInText = "SIGN IN";
            PinInstructionalText = "Enter your 4-digit PIN. If you have forgotten your PIN or want to sign in as a different user, select Forgot PIN.";
            IsEnabled = true;

            //if (InitialViewStateData is AuthenticateViewState.PinAuth)
            //    SetState(AuthenticateViewState.PinAuth);
            //else
            //    SetState(State);

            //Pin = new ValidatableObject<string>();

            //Pin.Rules.Add(new PinLimitValidationRule(){ MaxChar = 4 });  
        }

        private async void ExecuteSigninCommand()
        {
            IsBusy = true;
            
            try
            {
                //await SignInCommand();
                await SignInCommand();
            }
            catch (Exception ex) 
            {
                await Shell.Current.DisplayAlert("Error:", $"{ex.Message}", "Ok");
            }
            finally
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    IsBusy = false;
                });

            }
        }

        public override async Task OnNavigatingTo(Dictionary<string, object> parameter)
        {
            //if (parameter != null)
            //{
            //    //object initialState;

            //}
            if (parameter.TryGetValue("InitialViewState", out var stateTypeObject) && stateTypeObject is AuthenticateViewState.PinAuth)
            {
                SetState(AuthenticateViewState.PinAuth);
            }
            else
            {
                SetState(State);
            }
             await base.OnNavigatingTo(parameter);
        }

        public async Task OnAppearing()
        {
            Pin = string.Empty;
            if (LocalUser == null)
            {
                LocalUser = await _databaseService.GetUser();
            }
            bool userHasPin = !string.IsNullOrEmpty(LocalUser.EncryptedPinHash);
            if (userHasPin)
            {
                SetState(AuthenticateViewState.PinAuth);
            }
            else
            {
                SetState(State);
            }
        }

        


        //private void OnPropertyChanged()
        //{
        //    if (InitialViewStateData is AuthenticateViewState.PinAuth)
        //        SetState(AuthenticateViewState.PinAuth);
        //    else
        //        SetState(State);
        //}

        

        public void SetState(AuthenticateViewState state)
        {
            switch (state)
            {
                case AuthenticateViewState.AddPin:
                    Pin = string.Empty;
                    SignInText = "CREATE PIN";
                    Title = "CREATE PIN";
                    PlaceholderText = "Enter a 4-digit PIN";
                    TitleVisible = false;
                    ForgetPin = false;
                    IsEnabled = true;
                    LblPinHelpTextRow = 2;
                    PinInstructionalText = "Create your new 4-digit PIN to sign in to Ding! PIN should not be easy to guess, e.g 1111 or 1234.";
                    break;
                case AuthenticateViewState.ConfirmPin:
                    Pin = string.Empty;
                    SignInText = "CONFIRM PIN";
                    Title = "Re-enter PIN";
                    TitleVisible = false;
                    ForgetPin = false;
                    IsEnabled = true;
                    LblPinHelpTextRow = 2;
                    PlaceholderText = "Re-enter PIN";
                    PinInstructionalText = "Create your new 4-digit PIN to sign in to Ding! PIN should not be easy to guess, e.g 1111 or 1234.";
                    break;
                case AuthenticateViewState.PinAuth:
                    Pin = string.Empty;
                    TitleVisible = false;
                    SignInText = "SIGN IN";
                    PlaceholderText = "Enter your PIN";
                    ForgetPin = true;
                    IsEnabled = true;
                    LblPinHelpTextRow = 3;
                    PinInstructionalText = "Enter your 4-digit PIN. If you have forgotten your PIN or want to sign in as a different user, select Forgot PIN.";
                    break;
                case AuthenticateViewState.Congrats:
                    break;
                default:
                    break;
            }
            State = state;
        }


        public async Task CongratsCompleted()
        {
            try
            {
                if (LocalUser == null)
                {
                    LocalUser = await _databaseService.GetUser();
                }
                await _pinManager.SavePin(LocalUser, PreviousPin.ToSecureString());
                await NavigateToWebViewPage();
                SetState(AuthenticateViewState.Congrats);
            }
            catch (Exception ex)
            {
                //HandleError(ex, "Completed");
                ShowAlert("Unexpeted Error", $"{ex.Message}");
                IsEnabled = true;
            }
        }

        private bool AuthCheck()
        {
            try
            {
                return AsyncHelper.RunSync(SPAuthenticator.Instance.IsAuthenticated);
            }
            catch (InvalidUsernameOrPasswordException)
            {
                throw;
            }
            catch (AccountDisabledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _analyticsService.ReportException(ex, "Login generic exception");
                return false;
            }
        }

        private async Task<bool> HandleAuthCheck()
        {
            try
            {
                bool authOk = AuthCheck();
                if (!authOk)
                {
                    await ShowConfirmationAsync("Account issue", "There is an issue with your account or your current session has expired, you will need to authenticate again");  // wait for them to acknowledge...
                }
                return authOk;
            }
            catch (InvalidUsernameOrPasswordException)
            {
                await ShowConfirmationAsync("Account issue", $"{Consts.InvalidUserNameOrPasswordMessage} from the sign in page");
                return false;
            }
            catch (AccountDisabledException)
            {
                await ShowConfirmationAsync("Account locked", Consts.LockedAccountMessage);
                return false;
            }
            catch (Exception)
            {
                await ShowConfirmationAsync("Account issue", "There is an issue with your account or your current session has expired, you will need to authenticate again");
                return false;
            }
        }

        private void ResetUserDetails()
        {
            AsyncHelper.RunSync(() => _databaseService.ClearTables());
            Barrel.Current.EmptyAll();
            _cookieService.ClearAllCookies();
        }

        //private bool ValidateBeforeSubmit()
        //{
        //    bool ret = false;
        //    string errMsg = "";

        //    Pin.Validate();

        //    bool pinField = Pin.IsValid;

        //    if (!pinField)
        //    {
        //        errMsg += "Pin cannot be more or less than 4 digits";
        //    }

        //    ret = pinField;

        //    if (!ret)
        //        ShowAlert("Error", errMsg);

        //    return ret;
        //}


        public void AddPinEntered(string pin)
        {
            _previousPin = pin;
            SetState(AuthenticateViewState.ConfirmPin);
        }

        public async void ConfirmPinEntered(string pin)
        {
            if (_previousPin != pin)
            {
                SetState(AuthenticateViewState.AddPin);
                ShowAlert("Error", "PIN does not match");
            }
            else
            {
                SetState(AuthenticateViewState.Congrats);
                await CongratsCompleted();
            }
        }

        public async void AuthPinEntered(string pin)
        {
            
            var localUser = await _databaseService.GetUser();
            if (_pinManager.VerifyPin(localUser, pin.ToSecureString()))
            {
                bool authOk = await HandleAuthCheck();
                if (!authOk)
                {
                    _userProfileService.ResetUserDetails();
                    NavigateToLoginPage();
                    return;
                }
                bool shouldClearCookies = Consts.ForceClearCookiesAfterUnlock;

                if (shouldClearCookies)
                {
                    _cookieService.ClearAllCookies();
                }
                _mobileDeviceService.ResetAppActivity();
                
                var userProfile = await _userProfileService.GetCurrentUserProfile();
                if (userProfile != null)
                {
                    if (_rosterService.HasRoster(userProfile))
                    {
                       await _rosterService.Load(userProfile, true);
                    }
                    _mobileDeviceService.RunBackgroundTasks(userProfile);
                }
                await NavigateToWebViewPage();
                PinAttempts = 0;
            }
            else
            {
                SetState(AuthenticateViewState.PinAuth);
                PinAttempts++;
                if (PinAttempts >= 5)
                {
                    await ShowConfirmationAsync("Error", "Too many attempts, you will need to login again and create a new pin.");
                    NavigateToLoginPage();
                    return;
                }
                ShowAlert("PIN incorrect.", "Error");
                IsEnabled = true;
            }
        }

        private async Task NavigateToWebViewPage()
        {
            var appShell = _serviceProvider.GetRequiredService<YarraTramsMobileMauiBlazor.AppShell>();
            Application.Current.MainPage = appShell;
            await Shell.Current.GoToAsync("//WebViewPage", true);
        }

        private async Task SignInCommand()
        {
            try
            {
                if (!string.IsNullOrEmpty(Pin) && IsValid)
                {
                    IsEnabled = false;
                    switch (State)
                    {
                        case AuthenticateViewState.AddPin:
                            AddPinEntered(Pin);
                            break;

                        case AuthenticateViewState.ConfirmPin:
                            ConfirmPinEntered(Pin);
                            break;

                        case AuthenticateViewState.PinAuth:
                            //await Task.Delay(200); // needed to add this otherwise the loading dialog does not show
                            AuthPinEntered(Pin);
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    ShowAlert("Simple PIN's are not allowed.", "Error");
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Unexpected Error", $"{ex.Message}");

                IsEnabled = true;
            }
        }

        private async void ForgotPinCommand(object obj)
        {
            try
            {
                _userProfileService.ResetUserDetails();
                //_localUser = null;
                LocalUser.EncryptedPinHash = null;
                await _databaseService.ClearTables();
                NavigateToLoginPage();
            }
            catch (Exception ex)
            {
                ShowAlert("Forgot Pin", $"{ex.Message}");
            }
        }

        private void NavigateToLoginPage()
        {
            var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
            Application.Current.MainPage = new NavigationPage(loginPage);
        }

        #endregion Methods


    }
}
