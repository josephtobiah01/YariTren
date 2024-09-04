using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.Messaging;
using System.Diagnostics;
using YarraTramsMobileMauiBlazor.ViewModels.WebView;

namespace YarraTramsMobileMauiBlazor.Views.WebView;

public partial class SimpleWebViewPage : ContentPage
{
    private SimpleWebViewPageViewModel _viewModel;
	public SimpleWebViewPage(SimpleWebViewPageViewModel vm)
	{
        InitializeComponent();
        this.BindingContext = vm;
        _viewModel = vm;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _viewModel.OnAppearing();
    }

    protected override async void OnDisappearing()
    {
        base.OnAppearing();
        _viewModel.OnDisappearing();
    }

    private void ContentPage_Loaded(object sender, EventArgs e)
    {
        var statusBarBehavior = this.Behaviors.OfType<StatusBarBehavior>().FirstOrDefault();
        if (statusBarBehavior != null)
        {
            statusBarBehavior.StatusBarColor = Colors.Gray;
            statusBarBehavior.StatusBarStyle = StatusBarStyle.DarkContent;
        }
    }

    private void webView_Navigated(object sender, WebNavigatedEventArgs e)
    {
        Debug.WriteLine(e.Url.ToString());
    }

    private void webView_Navigating(object sender, WebNavigatingEventArgs e)
    {
        Debug.WriteLine(e.Url.ToString());
    }

}