using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Core;
using Data.SharePoint.Authentication;
using DevExpress.Maui.Controls;
using Java.Lang;
using Microsoft.Identity.Client;
using Plugin.CurrentActivity;
using YarraTramsMobileMauiBlazor.Services;
using YarraTramsMobileMauiBlazor.ViewModels.Login;
using YarraTramsMobileMauiBlazor.Views.Login;
using Microsoft.Maui.Controls;


namespace YarraTramsMobileMauiBlazor
{
    [Activity(Label = "Ding!", Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        internal static MainActivity Instance { get; private set; }
        private LoginPageViewModel vm => ServiceLocator.Current?.GetService<LoginPageViewModel>();
        const int RequestReadExternalStorageId = 0;
        private static string RedirectUrl()
        {
            string AppId = "au.com.yarratrams.employeeapp";
            return $"msauth://{AppId}/{Consts.AndroidAuthSignatureHash}";
        }

        protected override async void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            await DeviceInstallationService.RegisterDevice("infexiousdevhubstandard", "Endpoint=sb://infexiousdevhubstandard.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=Cwdp9bfz64MFTVMhjuX3M8kUPLIfoFF6thioL0GBqGI=");

            HandleIntent(Intent);

            CheckAndPromptForNotifications();

            //if (Intent?.Extras != null && Intent.Extras.ContainsKey("navigateTo"))
            //{
            //    string navigateTo = Intent.GetStringExtra("navigateTo");

            //    if (navigateTo == "WebViewPage")
            //    {
            //        MainThread.BeginInvokeOnMainThread(async () =>
            //        {
            //            // Get the MAUI App instance
            //            var mauiApp = (App)Microsoft.Maui.Controls.Application.Current;

            //            // Set the MainPage to AppShell (if needed)
            //            if (mauiApp.MainPage is not AppShell)
            //            {
            //                mauiApp.MainPage = ServiceLocator.Current.GetService<AppShell>();
            //            }
            //            AppShell appshell = new AppShell();
            //            appshell.ResetShellState();
            //            // Navigate to the WebViewPage
            //            await Shell.Current.GoToAsync("//WebViewPage", true);
            //        });
            //    }
            //}


            CrossCurrentActivity.Current.Init(this, savedInstanceState);

            // Set the current activity for MSAL
            PlatformConfig.Instance.ParentWindow = this;
            PlatformConfig.Instance.RedirectUri = RedirectUrl();

            RequestStoragePermission();
        }

        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            HandleIntent(intent);
        }

        private void HandleIntent(Intent? intent)
        {
            string? notificationPage = intent?.GetStringExtra("NotificationPage");
            if (notificationPage != null)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    var mauiApp = (App)Microsoft.Maui.Controls.Application.Current;

                    if (mauiApp.MainPage is not AppShell)
                    {
                        mauiApp.MainPage = ServiceLocator.Current?.GetService<AppShell>();
                    }

                    await Shell.Current.GoToAsync("//WebViewPage", true);
                });
            }
        }

        public override void OnBackPressed()
        {
            try
            {
                var mainPage = Microsoft.Maui.Controls.Application.Current.MainPage;
                if (mainPage is AppShell appShell)
                {
                    appShell.ResetShellState();
                }

                if (mainPage is NavigationPage navigationPage)
                {
                    var currentPage = navigationPage.CurrentPage;

                    if (currentPage is LoginPage)
                    {
                        base.OnBackPressed();
                    }
                    else
                    {
                        ShowLogoutPopup();
                    }
                }
                else
                {
                    // Handle the case where MainPage is not a NavigationPage, if necessary
                    ShowLogoutPopup();
                }
            }
            catch (IllegalArgumentException ex)
            {
                // Handle the exception
                System.Diagnostics.Debug.WriteLine($"Exception caught: {ex.Message}");
            }
        }

        private void ShowLogoutPopup()
        {
            var popup = new DXPopup
            {
                WidthRequest = 300,
                HeightRequest = 200
            };

            var layout = new StackLayout
            {
                Spacing = 10,
                Padding = new Thickness(10),
                Children =
                    {
                        new Label { Text = "Do you really want to Log Out?", FontSize = 18, HorizontalOptions = LayoutOptions.Center },
                        new Button { Text = "Yes", Command = new Command(() => OnLogoutConfirmed(popup)), HorizontalOptions = LayoutOptions.Center },
                        new Button { Text = "No", Command = new Command(() => OnLogoutCancelled(popup)), HorizontalOptions = LayoutOptions.Center }
                    }
            };

            popup.Content = layout;
            popup.Show();
        }

        private void OnLogoutConfirmed(DXPopup popup)
        {
            
            popup.IsOpen = false;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Microsoft.Maui.Controls.Application.Current.MainPage = new NavigationPage(new LoginPage(vm));
            });
        }

        private void OnLogoutCancelled(DXPopup popup)
        {
            popup.IsOpen = false;
        }

        void RequestStoragePermission()
        {
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) != (int)Permission.Granted)
            {
                // Permission is not granted. If necessary, request the permission.
                ActivityCompat.RequestPermissions(this, new string[] { Manifest.Permission.ReadExternalStorage }, RequestReadExternalStorageId);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == RequestReadExternalStorageId)
            {
                if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                {
                    // Permission granted. Continue with your operation.
                }
                else
                {
                    // Permission denied. Handle the case.
                }
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(requestCode, resultCode, data);
        }
        private void CheckAndPromptForNotifications()
        {
            NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.O && !notificationManager.AreNotificationsEnabled())
            {
                // Prompt the user to enable notifications
                ShowNotificationPrompt();
            }
        }


        private void ShowNotificationPrompt()
        {
            var popup = new DXPopup
            {
                WidthRequest = 100,
                HeightRequest = 200,
                BackgroundColor = Colors.White
            };

            var layout = new StackLayout
            {
                Spacing = 10,
                Padding = new Thickness(10),
                Children =
                {
                    new Label
                    {
                        Text = "Enable Notifications",
                        FontSize = 18,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.Center,
                        TextColor = Colors.Black
                    },
                    new Label
                    {
                        Text = "Notifications are disabled for this app. Please enable them in your device settings.",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        TextColor = Colors.Black
                    },
                    new Button
                    {
                        Text = "Go to Settings",
                        Command = new Command(() =>
                        {
                            OpenNotificationSettings();
                            popup.IsOpen = false;
                        }),
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                        TextColor = Colors.Black
                    }
                }
            };

            popup.Content = layout;
            popup.Show();
        }

        private void OpenNotificationSettings()
        {
            Intent intent = new Intent();
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                intent.SetAction(Android.Provider.Settings.ActionAppNotificationSettings);
                intent.PutExtra(Android.Provider.Settings.ExtraAppPackage, PackageName);
            }
            else
            {
                intent.SetAction(Android.Provider.Settings.ActionApplicationDetailsSettings);
                intent.AddCategory(Intent.CategoryDefault);
                intent.SetData(Android.Net.Uri.Parse("package:" + PackageName));
            }
            intent.AddFlags(ActivityFlags.NewTask);
            StartActivity(intent);
        }

    }
}
