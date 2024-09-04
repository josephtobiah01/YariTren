using DevExpress.Data.Printing.Native;
using DevExpress.Maui.Controls;
using YarraTramsMobileMauiBlazor.ViewModels.Roster;

namespace YarraTramsMobileMauiBlazor.Views.Roster;

public partial class RosterSimplePopup : DXPopup
{
	public RosterSimplePopup(
        RosterSimplePopupViewModel vm,
        string text, 
        DateTime selectedDate)
	{
		InitializeComponent();
        this.BindingContext = vm;
        message.Text = text;
        PopupTitle.Text = selectedDate.ToString("MMMM dd, yyyy");
    }
}