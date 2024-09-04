using YarraTramsMobileMauiBlazor.ViewModels.YourSay;

namespace YarraTramsMobileMauiBlazor.Views.YourSay;

public partial class YourSayFormViewPage : ContentPage
{
	public YourSayFormViewPage(YourSayFormViewPageViewModel vm)
	{
		InitializeComponent();
        this.BindingContext = vm;
	}

    

    void Provide_Feedback_Title_Handle_TextChanged(object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        if (((Entry)sender).Text != null)
        {
            providefeedbackTitleRemaining.Text = string.Format("{0} characters remaining", 40 - ((Entry)sender).Text.Length);
        }
    }

    void Provide_Feedback_Details_Handle_TextChanged(object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        if (((Editor)sender).Text != null)
        {
            providefeedbackDetailsRemaining.Text = string.Format("{0} characters remaining", 2000 - ((Editor)sender).Text.Length);
        }
    }
}