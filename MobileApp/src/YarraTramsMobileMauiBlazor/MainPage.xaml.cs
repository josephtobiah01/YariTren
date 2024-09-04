

namespace YarraTramsMobileMauiBlazor
{
    public partial class MainPage : ContentPage
    {
        public static MainPage Instance { get; private set; }
        public MainPage()
        {
            InitializeComponent();
            Instance = this;
        }

        public void ShowLoading()
        {
            LoadingOverlay.Show();
        }

        public void HideLoading()
        {
            LoadingOverlay.Hide();
        }
    }
}
