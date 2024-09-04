using YarraTramsMobileMauiBlazor.ViewModels.Login;

namespace YarraTramsMobileMauiBlazor.Views.Login;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginPageViewModel vm)
	{
		InitializeComponent();
		this.BindingContext = vm;
	}

    protected override void OnAppearing()
    {
        this.dxPopup.IsOpen = false;
        base.OnAppearing();
    }

    protected override void OnDisappearing()
    {
        this.dxPopup.IsOpen = true;
        base.OnDisappearing();
    }
}