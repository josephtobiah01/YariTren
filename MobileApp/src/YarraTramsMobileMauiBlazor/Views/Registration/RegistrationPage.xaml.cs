using YarraTramsMobileMauiBlazor.ViewModels.Registration;

namespace YarraTramsMobileMauiBlazor.Views.Registration
{
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage(RegistrationPageViewModel vm)
        {
            InitializeComponent();
            this.BindingContext = vm;
            try
            {
                NavigationPage.SetHasNavigationBar(this, false);
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
            }
        }
    }
}
