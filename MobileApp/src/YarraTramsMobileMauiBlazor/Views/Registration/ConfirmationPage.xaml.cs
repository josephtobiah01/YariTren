using YarraTramsMobileMauiBlazor.ViewModels.Registration;
namespace YarraTramsMobileMauiBlazor.Views.Registration
{
    public partial class ConfirmationPage : ContentPage
    {
        public ConfirmationPage(ConfirmationPageViewModel vm)
        {
            InitializeComponent();
            this.BindingContext = vm;
            NavigationPage.SetBackButtonTitle(this, "");
            NavigationPage.SetHasNavigationBar(this, false);
        }
    }
}
