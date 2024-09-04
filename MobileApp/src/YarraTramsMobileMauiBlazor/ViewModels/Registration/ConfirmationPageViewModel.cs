using Core.Interfaces;
using YarraTramsMobileMauiBlazor.Interfaces;
using YarraTramsMobileMauiBlazor.Views.Login;

namespace YarraTramsMobileMauiBlazor.ViewModels.Registration
{
    public class ConfirmationPageViewModel : ViewModelBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAnalyticsService _analyticsService;
        public ConfirmationPageViewModel(INavigationService navigationService, 
                                     IServiceProvider serviceProvider,
                                     IAnalyticsService analyticsService) : base(navigationService)
        {
            _serviceProvider = serviceProvider;
            _analyticsService = analyticsService;
        }

        public Command ReturnToLogin
        {
            get
            {
                return new Command(async () =>
                {
                    try
                    {
                        //var page = _serviceProvider.GetRequiredService<LoginPage>();
                        //Application.Current.MainPage = page;
                        await NavigationService.GoBackToRoot();
                    }
                    catch (Exception ex)
                    {
                        await ShowAlertAsync("Unexpected error", "An unexpected error occurred, please try again later.");
                        _analyticsService.ReportException(ex);
                    }
                });
            }
        }
    }
}
