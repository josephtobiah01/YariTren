using CommunityToolkit.Maui;
using Core;
using Core.Analytics;
using Core.Database;
using Core.Interfaces;
using Data.Services;
using Data.SharePoint.Clients;
using Data.SharePoint.Services;
using DevExpress.Maui;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter;
using Microsoft.Extensions.Logging;
using MonkeyCache.FileStore;
using SharedLogic;
using YarraTramsMobileMauiBlazor.CustomHandlers;
using YarraTramsMobileMauiBlazor.Interfaces;
using YarraTramsMobileMauiBlazor.Services;
using YarraTramsMobileMauiBlazor.ViewModels.AppShell;
using YarraTramsMobileMauiBlazor.ViewModels.Home;
using YarraTramsMobileMauiBlazor.ViewModels.Login;
using YarraTramsMobileMauiBlazor.ViewModels.Roster;
using YarraTramsMobileMauiBlazor.ViewModels.WebView;
using YarraTramsMobileMauiBlazor.ViewModels.YourSay;
using YarraTramsMobileMauiBlazor.Views.Login;
using YarraTramsMobileMauiBlazor.Views.Roster;
using YarraTramsMobileMauiBlazor.Views.WebView;
using YarraTramsMobileMauiBlazor.Views.YourSay;
using YarraTramsMobileMauiBlazor.Views.Registration;
using YarraTramsMobileMauiBlazor.ViewModels.Registration;
using YarraTramsMobileMauiBlazor.Views.Flyout;
using YarraTramsMobileMauiBlazor.ViewModels.Flyout;
using Microsoft.Maui.LifecycleEvents;


namespace YarraTramsMobileMauiBlazor
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseDevExpress()
                .UseDevExpressControls()
                .UseMauiCommunityToolkit()
                .ConfigureMauiHandlers((handlers) =>
                {
#if ANDROID
                    handlers.AddHandler(typeof(CustomEntry), typeof(Platforms.Android.ControlHandlers.CustomEntryHandler));
                    handlers.AddHandler(typeof(HybridWebView), typeof(Platforms.Android.ControlHandlers.HybridWebViewHandler));
#elif IOS
                    handlers.AddHandler(typeof(CustomEntry), typeof(Platforms.iOS.ControlHandlers.CustomEntryHandler));
                    handlers.AddHandler(typeof(HybridWebView), typeof(Platforms.iOS.ControlHandlers.HybridWebViewHandler));
#endif
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("HelveticaNeue67-MediumCondensed.otf", "HelveticaNeue67-MediumCondensed");
                    fonts.AddFont("HelveticaNeue67-MediumCondensedOblique.otf", "HelveticaNeue67-MediumCondensedOblique"); 
                    fonts.AddFont("HelveticaNeue97-BlackCondensed.otf", "HelveticaNeue97-BlackCondensed");
                    fonts.AddFont("HelveticaNeue97-BlackCondensedOblique.otf", "HelveticaNeue97-BlackCondensedOblique");
                    fonts.AddFont("HelveticaNeue-CondensedBold.ttf", "HelveticaNeue-CondensedBold");
                    fonts.AddFont("HelveticaNeue-Italic.ttf", "HelveticaNeue-Italic");
                    fonts.AddFont("HelveticaNeue-Regular.otf", "HelveticaNeue-Regular");
                    fonts.AddFont("Montserrat-Bold.ttf", "Montserrat-Bold");
                    fonts.AddFont("Montserrat-ExtraBold.ttf", "Montserrat-ExtraBold");
                    fonts.AddFont("Montserrat-Light.ttf", "Montserrat-Light");
                    fonts.AddFont("Montserrat-Medium.ttf", "Montserrat-Medium");
                    fonts.AddFont("Montserrat-Regular.ttf", "Montserrat-Regular");
                    fonts.AddFont("Montserrat-SemiBold.ttf", "Montserrat-SemiBold");
                    fonts.AddFont("faregular.ttf", "FAR");
                    fonts.AddFont("fasolid.ttf", "FAS");
                    fonts.AddFont("fabrands.ttf", "FAB");
                });

            Barrel.ApplicationId = "YarraTramsMobileMauiBlazor";
            SetupServices(builder.Services);
            RegisterNavigation();

            builder.Services.AddSingleton<AppShell>();

            //LOGINPAGE
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginPageViewModel>();
            //PINPAGE
            builder.Services.AddTransient<PinPage>();
            builder.Services.AddTransient<PinPageViewModel>();
            //HOMEPAGE
            builder.Services.AddTransient<HomePageViewModel>();
            //WebViewPage
            builder.Services.AddTransient<WebViewPage>();
            builder.Services.AddTransient<WebViewPageViewModel>();

            //SimpleWebViewPage
            builder.Services.AddTransient<SimpleWebViewPage>();
            builder.Services.AddTransient<SimpleWebViewPageViewModel>();

            builder.Services.AddSingleton<RosterViewPage>();
            builder.Services.AddSingleton<RosterViewPageViewModel>();

            builder.Services.AddTransient<YourSayViewPage>();
            builder.Services.AddTransient<YourSayViewPageViewModel>();

            builder.Services.AddTransient<YourSayFormViewPage>();
            builder.Services.AddTransient<YourSayFormViewPageViewModel>();

            builder.Services.AddTransient<ConfirmationViewPage>();
            builder.Services.AddTransient<ConfirmationViewPageViewModel>();

            builder.Services.AddTransient<RegistrationPage>();
            builder.Services.AddTransient<RegistrationPageViewModel>();

            builder.Services.AddTransient<ConfirmationPage>();
            builder.Services.AddTransient<ConfirmationPageViewModel>();

            builder.Services.AddTransient<CantLoginViewPage>();
            builder.Services.AddTransient<CantLoginViewPageViewModel>();

            builder.Services.AddTransient<FlyoutContentPage>();
            builder.Services.AddTransient<FlyoutContentPageViewModel>();
            //builder.Services.AddTransient<RosterDetailsPopupPage>();
            //builder.Services.AddTransient<RosterDetailsPopupPageViewModel>();

            //builder.Services.AddTransient<RosterSimplePopup>();
            //builder.Services.AddTransient<RosterSimplePopupViewModel>();

            //APPSHELL
            builder.Services.AddTransient<AppShellViewModel>();

            AppCenter.Start(Consts.AppCentreSecret, typeof(Analytics), typeof(Crashes));

            var app = builder.Build();

            // Set the service provider to the service locator
            ServiceLocator.Current = app.Services;

            return app;
        }

        private static void SetupServices(IServiceCollection services)
        {
            //services.AddSingleton<SPAuthenticator>();
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IApiManager, ApiManager>();
            services.AddSingleton<IClientManager, ClientManager>();
            services.AddSingleton<ICookieService, CookieService>();
            services.AddSingleton<IAnalyticsService, AnalyticsService>();
            services.AddSingleton<IPinManager, PinManager>();
            services.AddSingleton<IDatabaseService, DatabaseService>();
            services.AddSingleton<IUserProfileService, UserProfileService>();
            services.AddSingleton<IHttpUtility, HttpUtility>();
            services.AddSingleton<IConfigurationService, ConfigurationService>();
            services.AddSingleton<INewsService, NewsService>();
            services.AddSingleton<IAlertsService, AlertsService>();
            services.AddSingleton<IMobileDeviceService, MobileDeviceService>();
            services.AddSingleton<IRosterService, RosterService>();
            services.AddSingleton<ISQLiteFactory, SQLiteFactoryService>();
            services.AddSingleton<IYourSayService, YourSayService>();
            services.AddSingleton<IWebViewService, WebViewService>();
            services.AddSingleton<IUtilitiesService, UtilitiesService>();

            ////IClientBase
            //services.AddSingleton<IClientBase<Broadcast>, ClientBase<Broadcast>>();
            //services.AddSingleton<IClientBase<ConfigItem>, ClientBase<ConfigItem>>();
            //services.AddSingleton<IClientBase<UserProfile>, ClientBase<UserProfile>>();

            //if (DeviceInfo.Platform == DevicePlatform.Android || DeviceInfo.Platform == DevicePlatform.iOS || DeviceInfo.Platform == DevicePlatform.MacCatalyst)
            //{
            //    services.AddSingleton<ICustomService, CustomService>();
            //}



            //services.AddSingleton<ISQLiteFactory, SQLiteFactory>();
            //services.AddSingleton<IDatabaseService>(sp =>
            //{
            //    var sqliteFactory = sp.GetRequiredService<ISQLiteFactory>();
            //    return new DatabaseService(sqliteFactory);
            //});
            //services.AddSingleton<ICookieService, CookieService>();
            //services.AddSingleton<IRosterProvider, RosterProvider>();
        }


        private static void RegisterNavigation()
        {
            RegisterForNavigation.Register<LoginPage>();
            RegisterForNavigation.Register<PinPage>();
            RegisterForNavigation.Register<WebViewPage>();
            RegisterForNavigation.Register<SimpleWebViewPage>();
            RegisterForNavigation.Register<RosterViewPage>();
            RegisterForNavigation.Register<YourSayViewPage>();
            RegisterForNavigation.Register<YourSayFormViewPage>();
            RegisterForNavigation.Register<ConfirmationViewPage>();
            RegisterForNavigation.Register<RegistrationPage>();
            RegisterForNavigation.Register<ConfirmationPage>();
            RegisterForNavigation.Register<CantLoginViewPage>();
        }

    }
}
