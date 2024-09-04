using YarraTramsMobileMauiBlazor.ViewModels.Login;

namespace YarraTramsMobileMauiBlazor.Views.Login;

public partial class PinPage : ContentPage
{
    const int MAXPIN = 4;
    private PinPageViewModel _viewModel;
    public PinPage(PinPageViewModel vm)
	{
		 InitializeComponent();
		this.BindingContext = vm;
        _viewModel = vm;
    }

    void Entry_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue.Length > MAXPIN)
        {
            entryInput.Text = entryInput.Text.Remove(entryInput.Text.Length - 1);
        }
    }

    protected override async void OnAppearing()
    {
        this.dxPopup.IsOpen = false;
        await _viewModel.OnAppearing();
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        this.dxPopup.IsOpen = true;
        base.OnDisappearing();
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        this.dxPopup.IsOpen = false;
        base.OnNavigatedFrom(args);
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        this.dxPopup.IsOpen = false;
        base.OnNavigatedTo(args);
    }

    //protected override void OnDisappearing()
    //{
    //    this.activityIndicator.IsRunning = false;
    //    base.OnDisappearing(); 
    //}

    //protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    //{
    //    this.activityIndicator.IsRunning = true;
    //    base.OnNavigatingFrom(args);
    //}
}