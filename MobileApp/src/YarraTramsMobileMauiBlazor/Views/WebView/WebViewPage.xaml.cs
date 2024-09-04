
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.PlatformConfiguration;
using YarraTramsMobileMauiBlazor.ViewModels.WebView;

namespace YarraTramsMobileMauiBlazor.Views.WebView;

public partial class WebViewPage : ContentPage
{
	public WebViewPage(WebViewPageViewModel vm)
	{
		InitializeComponent();
        this.BindingContext = vm;

        //webView.RegisterNavigatingAction(OnWebViewNavigating);
        //webView.RegisterNavigatedAction(OnWebViewNavigated);
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        ApplyStatusBarBehavior();
    }

    public void ApplyStatusBarBehavior()
    {
        var statusBarBehavior = this.Behaviors.OfType<StatusBarBehavior>().FirstOrDefault();
        if (statusBarBehavior != null)
        {
            statusBarBehavior.StatusBarColor = Colors.Gray;
            statusBarBehavior.StatusBarStyle = StatusBarStyle.DarkContent;
        }
    }

    protected override void OnDisappearing()
    {
        this.dxPopup.IsOpen = true;
        this.activityIndicator.IsRunning = true;
        base.OnDisappearing();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        this.dxPopup.IsOpen = false;
        this.activityIndicator.IsRunning = false;

    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        this.dxPopup.IsOpen = false;
        base.OnNavigatedFrom(args);
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        this.dxPopup.IsOpen = false;
        this.activityIndicator.IsRunning = false;
        ApplyStatusBarBehavior();
        var vm = BindingContext as WebViewPageViewModel;
        if (vm != null)
        {
            vm.RestoreWebViewState();
        }
        base.OnNavigatedTo(args);
    }
    

    private async void EvalButton_Clicked(object sender, EventArgs e)
    {
        //await MyWebView.EvaluateJavaScriptAsync(new EvaluateJavaScriptAsyncRequest("nativeDemand('" + ChangeText.Text + "')")); 
        webView.Reload();
    }

    public void ResetState()
    {
        Shell.Current.FlyoutIsPresented = false;
        // Reload the WebView content
        webView?.Reload();

        // Reset the ViewModel's state
        var vm = BindingContext as WebViewPageViewModel;
        vm?.ResetState();

        // Reset the popup and activity indicator
        this.dxPopup.IsOpen = false;

        // Reapply status bar behavior
        ApplyStatusBarBehavior();
    }
}