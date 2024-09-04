using Core;
using Core.Analytics;
using Core.Exceptions;
using Core.Helpers;
using Core.Interfaces;
using Data.SharePoint.Authentication;
using DevExpress.Maui.Controls;
using MonkeyCache.FileStore;
using YarraTramsMobileMauiBlazor.Interfaces;
using YarraTramsMobileMauiBlazor.Views.Login;
using YarraTramsMobileMauiBlazor.Views.Registration;

namespace YarraTramsMobileMauiBlazor.ViewModels.Registration
{
    public class RegistrationPageViewModel : ViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAnalyticsService _analyticsService;

        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Phone { get; set; } = "";

        public RegistrationPageViewModel(INavigationService navigationService,
                                    IServiceProvider serviceProvider,
                                    IAnalyticsService analyticsService) : base(navigationService)
        {
            _serviceProvider = serviceProvider;
            _analyticsService = analyticsService;
        }

        public Command RegisterCommand
        {
            get
            {
                return new Command(async () =>
                {
                    try
                    {
                        if (string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Phone))
                        {
                            await ShowAlertAsync("Error", "Please enter Name, Email and Phone for registration");
                            return;
                        }

                        // Attempt to save. The following is intended to show a dialog, submit the form, hide the page while we are poppping the form page, prior to the form submission confirmation page being shown...
                        //_dialogs.ShowLoading("Submitting...", null);
                        bool success = await RegisterForDingCommunityAsync();
                        if (!success)
                        {
                            await ShowAlertAsync("Registration Error", "There was one or more issues with registration");
                            await NavigateToLoginPage();
                            return;
                        }
                        await NavigateToConfirmationPage();
                    }
                    catch (Exception ex)
                    {
                        await ShowAlertAsync("Unexpected error", "An unexpected error occurred, please try again later.");
                        _analyticsService.ReportException(ex);
                        await NavigateToLoginPage();
                    }
                    finally
                    {
                        //_dialogs.HideLoading();
                    }
                });
            }
        }
        public Command TermsOfUseCommand
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

        public Command GoBack
        {
            get
            {
                return new Command(async () =>
                {
                    try
                    {
                        await NavigationService.GoBack();
                    }
                    catch (Exception ex)
                    {
                        await ShowAlertAsync("Unexpected error", "An unexpected error occurred, please try again later.");
                        _analyticsService.ReportException(ex);
                    }
                });
            }

        }


        private async Task NavigateToLoginPage()
        {
            //var page = _serviceProvider.GetRequiredService<LoginPage>();
            //Application.Current.MainPage = page;
            await NavigationService.GoBack(); // Pop's the current page....
        }

        private async Task NavigateToConfirmationPage()
        {
            //var page = _serviceProvider.GetRequiredService<ConfirmationPage>();
            //Application.Current.MainPage = page;
            await NavigationService.NavigateTo(nameof(ConfirmationPage));
        }

        async Task<bool> RegisterForDingCommunityAsync()
        {
            bool success = false;
            await Task.Run(() =>
              {
                  success = RegistrationHelper.RegisterUser(Name, Email, Phone);
              });
            return success;
        }




    }
}
