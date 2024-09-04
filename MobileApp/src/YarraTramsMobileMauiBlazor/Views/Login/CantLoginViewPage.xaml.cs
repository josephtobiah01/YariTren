using YarraTramsMobileMauiBlazor.ViewModels.Login;

namespace YarraTramsMobileMauiBlazor.Views.Login;

public partial class CantLoginViewPage : ContentPage
{
	public CantLoginViewPage(CantLoginViewPageViewModel vm)
	{
		InitializeComponent();
		this.BindingContext = vm;
	}
}