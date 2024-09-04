using Core.Models;
using YarraTramsMobileMauiBlazor.ViewModels.Roster;

namespace YarraTramsMobileMauiBlazor.Views.Roster;

public partial class RosterViewPage : ContentPage
{
    //private RosterViewPageViewModel? _viewModel;


    public RosterViewPage(RosterViewPageViewModel vm)
	{
		InitializeComponent();
        this.BindingContext = vm;

        //_viewModel = vm;
    }

    protected override void OnAppearing()
    {
        this.activityIndicator.IsRunning = true;
        base.OnAppearing();
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        this.activityIndicator.IsRunning = false;
        base.OnNavigatedTo(args);
    }

    //private void ContentPage_Loaded(object sender, EventArgs e)
    //{
    //    if (this.BindingContext is RosterViewPageViewModel viewmodel)
    //    {
    //        viewmodel.OnPageLoaded();
    //    }
    //}

    //protected override void OnAppearing()
    //{
    //    base.OnAppearing();

    //    // Example: You should pass the actual RosterRecord and selectedDate
    //    RosterRecord record = new RosterRecord(); // Get this from somewhere
    //    DateTime selectedDate = DateTime.Now; // Get this from somewhere

    //    _viewModel?.Initialize(record, selectedDate);

}
