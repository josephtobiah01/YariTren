using Core.Models;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using YarraTramsMobileMauiBlazor.ViewModels.Roster;

namespace YarraTramsMobileMauiBlazor.Views.Roster;

public partial class RosterWeeklyViewpage : ContentView
{
    DataTemplate? DataTemplateWeek;
    private readonly RosterViewPageViewModel _rosterViewPageViewModel;
    public RosterWeeklyViewpage()
	{
		InitializeComponent();

        DataTemplateWeek = this.Resources["weekTemplate"] as DataTemplate;
        lstWeekItem.ItemTemplate = DataTemplateWeek;
        lstWeekItem.ItemTapped += LstWeekItem_ItemTapped;

        _rosterViewPageViewModel = (RosterViewPageViewModel)Application.Current?.MainPage?.Handler?.MauiContext?.Services.GetService(typeof(RosterViewPageViewModel))!;
        this.BindingContext = _rosterViewPageViewModel;

        var leftSwipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Left };
        leftSwipeGesture.Swiped += OnSwiped;
        var rightSwipeGesture = new SwipeGestureRecognizer { Direction = SwipeDirection.Right };
        rightSwipeGesture.Swiped += OnSwiped;

        lstWeekItem.GestureRecognizers.Add(leftSwipeGesture);
        lstWeekItem.GestureRecognizers.Add(rightSwipeGesture);
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == "WeekListItems")
        {
            if (WeekListItems != null)
                lstWeekItem.ItemsSource = WeekListItems;
        }
    }

    void LstWeekItem_ItemTapped(object? sender, ItemTappedEventArgs e)
    {
        try
        {
            lstWeekItem.SelectedItem = null;
            var item = e.Item as RosterRecord;
            item.IsExtended = !item.IsExtended;
            lstWeekItem.ItemTemplate = null;
            lstWeekItem.ItemTemplate = DataTemplateWeek;

        }
        catch (Exception ex)
        {
            Application.Current?.MainPage?.DisplayAlert("Error", $"{ex.Message}", "Ok");
        }
    }

    public static readonly BindableProperty IsWeekLeftArrowProperty = BindableProperty.Create(
        propertyName: "IsWeekLeftArrow",
        returnType: typeof(bool),
        declaringType: typeof(RosterWeeklyViewpage),
        defaultValue: default(bool));

    public bool IsWeekLeftArrow
    {
        get { return (bool)base.GetValue(IsWeekLeftArrowProperty); }
        set { base.SetValue(IsWeekLeftArrowProperty, value); }
    }

    public static readonly BindableProperty IsWeekRightArrowProperty = BindableProperty.Create(
        propertyName: "IsWeekRightArrow",
        returnType: typeof(bool),
        declaringType: typeof(RosterWeeklyViewpage),
        defaultValue: default(bool));

    public bool IsWeekRightArrow
    {
        get { return (bool)base.GetValue(IsWeekRightArrowProperty); }
        set { base.SetValue(IsWeekRightArrowProperty, value); }
    }

    public static readonly BindableProperty WeekListItemsProperty = BindableProperty.Create(
        propertyName: "WeekListItems",
        returnType: typeof(ObservableCollection<RosterRecord>),
        declaringType: typeof(RosterWeeklyViewpage),
        defaultValue: default(ObservableCollection<RosterRecord>));

    public ObservableCollection<RosterRecord> WeekListItems
    {
        get { return (ObservableCollection<RosterRecord>)base.GetValue(WeekListItemsProperty); }
        set { base.SetValue(WeekListItemsProperty, value); }
    }

    void WeekLeftArrow_Handle_Tapped(object sender, System.EventArgs e)
    {
        MoveLeft();
    }

    void WeekRightArrow_Handle_Tapped(object sender, System.EventArgs e)
    {
        MoveRight();
    }

    void OnSwiped(object? sender, SwipedEventArgs e)
    {
        switch (e.Direction)
        {
            case SwipeDirection.Left:
                //MoveRight();
                break;
            case SwipeDirection.Right:
                // MoveLeft();
                break;
            case SwipeDirection.Up:
                // Handle the swipe
                break;
            case SwipeDirection.Down:
                // Handle the swipe
                break;
        }
    }

    private void MoveLeft()
    {
        if (WeekLeftArrowCommand != null && WeekLeftArrowCommand.CanExecute(null))
        {
            WeekLeftArrowCommand.Execute(null);
        }
    }

    private void MoveRight()
    {
        if (WeekRightArrowCommand != null && WeekRightArrowCommand.CanExecute(null))
        {
            WeekRightArrowCommand.Execute(null);
        }
    }


    

    public static readonly BindableProperty WeekRightArrowCommandProperty = BindableProperty.Create(nameof(Command), typeof(Command), typeof(RosterWeeklyViewpage), null);
    public Command WeekRightArrowCommand
    {
        get { return (Command)GetValue(WeekRightArrowCommandProperty); }
        set { SetValue(WeekRightArrowCommandProperty, value); }
    }

    public static readonly BindableProperty WeekLeftArrowCommandProperty = BindableProperty.Create(nameof(Command), typeof(Command), typeof(RosterWeeklyViewpage), null);
    public Command WeekLeftArrowCommand
    {
        get { return (Command)GetValue(WeekLeftArrowCommandProperty); }
        set { SetValue(WeekLeftArrowCommandProperty, value); }
    }

    //private void ContentView_Loaded(object sender, EventArgs e)
    //{
    //    _rosterViewPageViewModel.
    //}
}