using CommunityToolkit.Mvvm.Messaging;
using YarraTramsMobileMauiBlazor.Views.Flyout;
using YarraTramsMobileMauiBlazor.Views.Login;
using YarraTramsMobileMauiBlazor.Views.Roster;
using YarraTramsMobileMauiBlazor.Views.WebView;
using YarraTramsMobileMauiBlazor.Views.YourSay;

namespace YarraTramsMobileMauiBlazor;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
		Routing.RegisterRoute(nameof(PinPage), typeof(PinPage));
		Routing.RegisterRoute(nameof(WebViewPage), typeof(WebViewPage));
        Routing.RegisterRoute(nameof(SimpleWebViewPage), typeof(SimpleWebViewPage));
        Routing.RegisterRoute(nameof(RosterViewPage), typeof(RosterViewPage));
		Routing.RegisterRoute(nameof(YourSayViewPage), typeof(YourSayViewPage));
		Routing.RegisterRoute(nameof(YourSayFormViewPage), typeof(YourSayFormViewPage));
		Routing.RegisterRoute(nameof(ConfirmationViewPage), typeof(ConfirmationViewPage));
		Routing.RegisterRoute(nameof(CantLoginViewPage), typeof(CantLoginViewPage));
    }

    public void ResetShellState()
    {
        Shell.Current.FlyoutIsPresented = false;
        // Reset the state of FlyoutContentPage.xaml
        var flyoutContentPage = this.FindByName<ContentView>("FlyoutContentView");
        if (flyoutContentPage != null && flyoutContentPage.Content is FlyoutContentPage flyoutPage)
        {
            flyoutPage.ResetState();
        }

        // Reset the state of WebViewPage.xaml
        var webViewPage = this.FindByName<ShellContent>("WebViewPage");
        if (webViewPage?.ContentTemplate?.CreateContent() is WebViewPage webView)
        {
            webView.ResetState();
        }
    }
}