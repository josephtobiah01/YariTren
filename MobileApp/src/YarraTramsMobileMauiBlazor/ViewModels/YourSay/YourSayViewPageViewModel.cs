using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTramsMobileMauiBlazor.Interfaces;
using YarraTramsMobileMauiBlazor.Views.YourSay;

namespace YarraTramsMobileMauiBlazor.ViewModels.YourSay
{

    public enum SelectionState { MakeSuggestion, ProvideFeedback, Compliment, Safety, None };
    public class YourSayViewPageViewModel : ViewModelBase
    {
        #region Fields

        private readonly INavigationService _navigationService;


        #endregion Fields


        public string Title { get; set; }
        public string TitleMessage { get; set; }
        public string TypeOfPageText { get; set; }
        public string MakeSuggestionPage { get; set; }
        public string ProvideFeedbackPage { get; set; }
        public string GiveComplimentPage { get; set; }
        public string ReportIncidentPage { get; set; }
        public string MakeSuggestionBoarder { get; set; }
        public string ProvideFeedbackBoarder { get; set; }
        public string ComplimentBoarder { get; set; }
        public string SafetyBoarder { get; set; }

        public Command MakeSuggestion { get; set; }
        public Command ProvideFeedback { get; set; }
        public Command GiveCompliment { get; set; } 
        public Command ReportIncident { get; set; }
        public Command GoBack { get; set; }

        public YourSayViewPageViewModel(
            INavigationService navigationService) : base(navigationService)
        {
            _navigationService = navigationService;

            Title = "How are we doing?";
            TitleMessage = "We all have a role to play in creating a better tram network for our city. Report a safety hazard, risk or suggestion, or give your feedback or ideas to help improve our network for our passengers.";
            TitleMessage = "We all have a role to play in creating a better tram network for our city. Provide your feedback or ideas to help improve our network for our passengers, or report a safety hazard, risk or suggestion";
            TypeOfPageText = "What would you like to do?";
            MakeSuggestionPage = "Make a suggestion";
            ProvideFeedbackPage = "Provide feedback";
            GiveComplimentPage = "Give a compliment";
            ReportIncidentPage = "Make a safety suggestion";
            SelectionHighLighted(SelectionState.None);

            MakeSuggestion = new Command(MakeSuggestionCommand);
            ProvideFeedback = new Command(ProvideFeedbackCommand);
            GiveCompliment = new Command(GiveComplimentCommand);
            ReportIncident = new Command(ReportIncidentCommand);
            GoBack = new Command(GoBackCommand);
        }

        private void GoBackCommand(object obj)
        {
            _navigationService.NavigateTo("//WebViewPage");
        }

        void SelectionHighLighted(SelectionState state)
        {
            switch (state)
            {
                case SelectionState.MakeSuggestion:
                    MakeSuggestionBoarder = "#CE0F69";
                    ProvideFeedbackBoarder = "#C0C0C0";
                    ComplimentBoarder = "#C0C0C0";
                    SafetyBoarder = "#C0C0C0";
                    break;
                case SelectionState.ProvideFeedback:
                    MakeSuggestionBoarder = "#C0C0C0";
                    ProvideFeedbackBoarder = "#CE0F69";
                    ComplimentBoarder = "#C0C0C0";
                    SafetyBoarder = "#C0C0C0";
                    break;
                case SelectionState.Compliment:
                    MakeSuggestionBoarder = "#C0C0C0";
                    ProvideFeedbackBoarder = "#C0C0C0";
                    ComplimentBoarder = "#CE0F69";
                    SafetyBoarder = "#C0C0C0";
                    break;
                case SelectionState.Safety:
                    MakeSuggestionBoarder = "#C0C0C0";
                    ProvideFeedbackBoarder = "#C0C0C0";
                    ComplimentBoarder = "#C0C0C0";
                    SafetyBoarder = "#CE0F69";
                    break;
                case SelectionState.None:
                    MakeSuggestionBoarder = "#C0C0C0";
                    ProvideFeedbackBoarder = "#C0C0C0";
                    ComplimentBoarder = "#C0C0C0";
                    SafetyBoarder = "#C0C0C0";
                    break;
                default:
                    break;
            }

        }

        private async void MakeSuggestionCommand(object obj)
        {
            try
            {
                SelectionHighLighted(SelectionState.MakeSuggestion);
                string formType = SelectionState.MakeSuggestion.ToString();
                //var yourSayFormPage = FreshPageModelResolver.ResolvePageModel<YourSayFormPageModel>(formType);
                //await CurrentPage.Navigation.PushAsync(yourSayFormPage, true);
                Dictionary<string, object> _param = new Dictionary<string, object>();

                _param.Add("FormType", formType);

                //await _navigationService.NavigateTo(nameof(YourSayFormViewPage), _param);
                await Shell.Current.GoToAsync(nameof(YourSayFormViewPage), _param);
                

            }
            catch (Exception ex)
            {
                ShowAlert("Error: ", $"{ex.Message}");
            }
        }

        private async void ProvideFeedbackCommand(object obj)
        {
            try
            {
                SelectionHighLighted(SelectionState.ProvideFeedback);
                string formType = SelectionState.ProvideFeedback.ToString();
                //var yourSayFormPage = FreshPageModelResolver.ResolvePageModel<YourSayFormPageModel>(formType);
                //await CurrentPage.Navigation.PushAsync(yourSayFormPage, true);
                Dictionary<string, object> _param = new Dictionary<string, object>();

                _param.Add("FormType", formType);

                await _navigationService.NavigateTo("YourSayFormViewPage", _param);
            }
            catch (Exception ex)
            {
                ShowAlert("Error: ", $"{ex.Message}");
            }
        }

        private async void GiveComplimentCommand(object obj)
        {
            try
            {
                SelectionHighLighted(SelectionState.Compliment);
                string formType = SelectionState.Compliment.ToString();
                //var yourSayFormPage = FreshPageModelResolver.ResolvePageModel<YourSayFormPageModel>(formType);
                //await CurrentPage.Navigation.PushAsync(yourSayFormPage, true);
                Dictionary<string, object> _param = new Dictionary<string, object>();

                _param.Add("FormType", formType);

                await _navigationService.NavigateTo("YourSayFormViewPage", _param);
            }
            catch (Exception ex)
            {
                ShowAlert("Error: ", $"{ex.Message}");
            }
        }

        private async void ReportIncidentCommand(object obj)
        {
            try
            {
                SelectionHighLighted(SelectionState.Safety);
                string formType = SelectionState.Safety.ToString();
                //var yourSayFormPage = FreshPageModelResolver.ResolvePageModel<YourSayFormPageModel>(formType);
                //yourSayFormPage.Title = "Report Incident";
                //await CurrentPage.Navigation.PushAsync(yourSayFormPage, true);
                Dictionary<string, object> _param = new Dictionary<string, object>();

                _param.Add("FormType", formType);
                _param.Add("YourSayFormViewPageTitle", "Report Incident");

                await _navigationService.NavigateTo("YourSayFormViewPage", _param);
            }
            catch (Exception ex)
            {
                ShowAlert("Error: ", $"{ex.Message}");
            }
        }

        
    }
}
