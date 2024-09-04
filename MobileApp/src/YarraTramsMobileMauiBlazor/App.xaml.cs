using Core.Database;
using Core.Helpers;
using Core.Interfaces;
using Data.Database;
using Data.SharePoint.Authentication;
using YarraTramsMobileMauiBlazor.Interfaces;
using YarraTramsMobileMauiBlazor.Services;
using YarraTramsMobileMauiBlazor.ViewModels.Login;
using YarraTramsMobileMauiBlazor.Views.Login;

namespace YarraTramsMobileMauiBlazor
{
    public partial class App : Application
    {
        public class Messages
        {
            public static string TabsMessage = "App.Tabs";
            public static string PushNotificationRecievedMessage = "App.PushNotificationRecieved";
            public static string SendDeviceInfo = "App.SendDeviceInfo";
            public static string PageNavigationMessage = "App.PageNavigation";
            public static string MenuCloseMessage = "App.MenuClose";
            public static string RosterMessage = "App.Roster";
            public static string SendUserDataMessage = "App.SendUserData";
            public static string RosterDataInProgressMessage = "App.RosterDataInProgressMessage";
            public const string NewsTab = "news";
            public const string MyToolsTab = "mytools";
        }

        private readonly IAnalyticsService _analyticsService;
        private readonly INavigationService _navigationService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDatabaseService _databaseService;
        private readonly IRosterService _rosterService;
        private readonly IUserProfileService _userProfileService;

        public static DateTime? LastUnlockTime;
        private const int AppLockTimeout = 40;

        public static Func<bool> IsJailBroken = null;
        public static Func<IEnumerable<string>> JailBreaks = null;

        public App(
            IAnalyticsService analyticsService,
            INavigationService navigationService,
            IServiceProvider serviceProvider,
            IDatabaseService databaseService,
            IRosterService rosterService,
            IUserProfileService userProfileService
            )
        {
            InitializeComponent();

            MainPage = new ContentPage();

            _analyticsService = analyticsService;
            _navigationService = navigationService;
            _serviceProvider = serviceProvider;
            _databaseService = databaseService;
            _rosterService = rosterService;
            _userProfileService = userProfileService;

            SetupDatabase();
            
        }
        private void SetupDatabase()
        {
            var sqliteFactory = _serviceProvider.GetRequiredService<ISQLiteFactory>();
            LocalDatabaseSetup.CreateTables(sqliteFactory);
        }

        protected override async void OnStart()
        {
            base.OnStart();
            await SetMainPageAsync();

        }

        protected override void OnSleep()
        {
            ResetAppActivity();
            base.OnSleep();
        }

        public static void ResetAppActivity()
        {
            LastUnlockTime = DateTime.Now;
        }

        //protected override async void OnResume()
        //{
        //    try
        //    {
        //        // Handle when your app resumes
        //        //TODO: If the user has been using the app for 10 minutes. the lock will occur immeidately.  May need to have a sliding scale.

        //        if (HasActivityExpired() && await HasUserProfile())
        //        {
        //            Dictionary<string, object> _param = new Dictionary<string, object>();

        //            _param.Add("ViewStateAddPin", AuthenticateViewState.AddPin);

        //            _navigationService.NavigateTo(nameof(PinPage), _param);
        //        }
        //        else if (HasActivityExpired())
        //        {
        //            _navigationService.NavigateTo(nameof(LoginPage));
        //            return;
        //        }
        //        //Roster data should be re-loaded for Tram Drivers in the background when the app is resumed
        //        // TODO: Only call the following *if* we are autenticated and/or we have a valid username and password saved:
        //        LoadRosterData();
        //    }
        //    catch (Exception ex)
        //    {
        //        _analyticsService.ReportException(ex);
        //    }
        //}



        public async Task<bool> HasUserProfile()
        {
            try
            {
                //bool hasProfile = false;
                //AsyncHelper.RunSync(async () =>
                //{
                //    var profile = await _databaseService.GetUser();
                //    hasProfile = profile != null && !string.IsNullOrEmpty(profile.AccessToken);
                //});
                //return hasProfile;
                var authenticated = await SPAuthenticator.Instance.IsAuthenticated();
                if (authenticated)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool HasActivityExpired()
        {
            return LastUnlockTime.HasValue && (DateTime.Now - LastUnlockTime.Value) > new TimeSpan(0, 0, AppLockTimeout); //TODO: move this to a server side setting at some point.
        }
        private async Task SetMainPageAsync()
        {
            var authenticated = await SPAuthenticator.Instance.IsAuthenticated();
            if (authenticated)
            {
                //await Shell.Current.GoToAsync("//PinPage");
                var pinPage = _serviceProvider.GetRequiredService<PinPage>();
                MainPage = pinPage;
            }
            else
            {
                // No token available silently, navigate to LoginPage
                //await Shell.Current.GoToAsync("//LoginPage");
                var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
                MainPage = new NavigationPage(loginPage);
            }
        }

        public async void LoadRosterData()
        {
            try
            {   
                var userProfile = await _userProfileService.GetCurrentUserProfile();
                if (_rosterService.HasRoster(userProfile))
                {
                    await _rosterService.Load(userProfile, true);
                }
            }
            catch (Exception ex)
            {
                _analyticsService.ReportException(ex, "reloading roster data from App.xaml.cs");
            }
        }

        public static IServiceProvider ServiceProvider =>
        Current.Handler?.MauiContext?.Services;
    }
}
