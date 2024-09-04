using YarraTramsMobileMauiBlazor.ViewModels.YourSay;

namespace YarraTramsMobileMauiBlazor.Views.YourSay;

public partial class ConfirmationViewPage : ContentPage
{
	public ConfirmationViewPage(ConfirmationViewPageViewModel vm)
	{
		InitializeComponent();
		this.BindingContext = vm;
	}
}