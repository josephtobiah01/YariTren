namespace YarraTramsMobileMauiBlazor.CustomControls;

public partial class LoadingOverlay : ContentView
{
	public LoadingOverlay()
	{
		InitializeComponent();
	}

    public void Show()
    {
        IsVisible = true;
    }

    public void Hide()
    {
        IsVisible = false;
    }
}