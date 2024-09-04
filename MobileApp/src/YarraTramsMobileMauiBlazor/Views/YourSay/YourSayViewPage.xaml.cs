using YarraTramsMobileMauiBlazor.ViewModels.YourSay;

namespace YarraTramsMobileMauiBlazor.Views.YourSay;

public partial class YourSayViewPage : ContentPage
{
	public YourSayViewPage(YourSayViewPageViewModel vm)
	{
		InitializeComponent();
		this.BindingContext = vm;
	}
}