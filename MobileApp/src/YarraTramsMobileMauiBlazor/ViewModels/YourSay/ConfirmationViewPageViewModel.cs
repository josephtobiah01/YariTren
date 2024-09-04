using Core.Interfaces;
using YarraTramsMobileMauiBlazor.Interfaces;
using YarraTramsMobileMauiBlazor.Views.WebView;

namespace YarraTramsMobileMauiBlazor.ViewModels.YourSay
{
    public class ConfirmationViewPageViewModel : ViewModelBase
    {

        private readonly IAnalyticsService _analyticsService;
        private readonly INavigationService _navigationService;
        


        public ConfirmationViewPageViewModel(
            INavigationService navigationService,
            IAnalyticsService analyticsService
            ) : base(navigationService)
        {
            _analyticsService = analyticsService;
            _navigationService = navigationService;
            

            TrackCustomUserAction("Your Say", "User submitted Your Say form");
        }


        private void TrackCustomUserAction(string area, string message)
        {
            var properties = new Dictionary<string, string> { { "Area", area }, { "Message", message } };
            _analyticsService.ReportEvent(message, properties);
            
        }

        public Command ConfirmationCommand
        {
            get
            {
                return new Command(async () =>
                {
                    try
                    {

                        await Shell.Current.Navigation.PopToRootAsync();
                        //await Shell.Current.GoToAsync("//WebViewPage", true);

                    }
                    catch (Exception ex)
                    {
                        ShowAlert("Error: ", $"{ex.Message}");
                    }
                });
            }
        }
    }
}
