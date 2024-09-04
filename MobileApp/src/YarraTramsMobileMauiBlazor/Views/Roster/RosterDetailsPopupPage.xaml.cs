using Core.Models;
using DevExpress.Maui.Controls;
using YarraTramsMobileMauiBlazor.ViewModels.Roster;

namespace YarraTramsMobileMauiBlazor.Views.Roster;

public partial class RosterDetailsPopupPage : DXPopup
{

    //public static readonly BindableProperty IsPopupOpenProperty =
    //        BindableProperty.Create(nameof(IsPopupOpen), typeof(bool), typeof(RosterDetailsPopupPage), false, propertyChanged: OnIsPopupOpenChanged);

    //public bool IsPopupOpen
    //{
    //    get => (bool)GetValue(IsPopupOpenProperty);
    //    set => SetValue(IsPopupOpenProperty, value);
    //}

    public RosterDetailsPopupPage(
        RosterDetailsPopupPageViewModel vm,
        RosterRecord model,
		DateTime selectedDate)
	{
		InitializeComponent();

        this.BindingContext = vm;
        //
        //PopupTitle.Text = selectedDate.ToString("MMMM dd, yyyy");
        //IsPopupOpen = true;
    }

    //private static async void OnIsPopupOpenChanged(BindableObject bindable, object oldValue, object newValue)
    //{
    //    var control = (RosterDetailsPopupPage)bindable;
    //    if ((bool)newValue)
    //    {
    //        await control.Content.FadeTo(1, 250);
    //    }
    //    else
    //    {
    //        await control.Content.FadeTo(0, 250);
    //    }
    //}

    //private void Close()
    //{
    //    IsPopupOpen = false;
    //}


    //private void SwipeGestureRecognizer_Swiped(System.Object sender, Microsoft.Maui.Controls.SwipedEventArgs e)
    //{
    //    switch (e.Direction)
    //    {
    //        case SwipeDirection.Left:
    //            Close();
    //            break;
    //        case SwipeDirection.Right:
    //            Close();
    //            break;
    //        case SwipeDirection.Up:
    //            // Handle the swipe
    //            break;
    //        case SwipeDirection.Down:
    //            // Handle the swipe
    //            break;
    //    }
    //}
}