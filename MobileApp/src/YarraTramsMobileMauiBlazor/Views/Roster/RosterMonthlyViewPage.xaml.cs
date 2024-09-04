using Core.Models;
using System.Globalization;
using System.Runtime.CompilerServices;
using YarraTramsMobileMauiBlazor.ViewModels.Roster;

namespace YarraTramsMobileMauiBlazor.Views.Roster;

public partial class RosterMonthlyViewPage : ContentView
{
    protected DateTime CurrentRosterDate { get; set; } = DateTime.Now;
    private readonly RosterViewPageViewModel _rosterViewPageViewModel;
    public RosterMonthlyViewPage()
	{
		InitializeComponent();

        //_rosterViewPageViewModel = rosterViewPageViewModel;
        //_rosterViewPageViewModel = App.ServiceProvider.GetService<RosterViewPageViewModel>();
        _rosterViewPageViewModel = (RosterViewPageViewModel)Application.Current?.MainPage?.Handler?.MauiContext?.Services.GetService(typeof(RosterViewPageViewModel))!;
        this.BindingContext = _rosterViewPageViewModel;

    }

    public static readonly BindableProperty DateClickedProperty = BindableProperty.Create(nameof(Command), typeof(Command), typeof(RosterMonthlyViewPage), null);
    public Command DateClicked
    {
        get { return (Command)GetValue(DateClickedProperty); }
        set { SetValue(DateClickedProperty, value); }
    }

    void Date_Handle_Tapped(object sender, System.EventArgs e)
    {
        if (DateClicked != null && DateClicked.CanExecute(null))
        {
            DateClicked.Execute(null);
        }
    }

    public static readonly BindableProperty RostersProperty = BindableProperty.Create(
       propertyName: "Rosters",
       returnType: typeof(List<RosterRecord>),
       declaringType: typeof(RosterMonthlyViewPage),
       defaultValue: default(List<RosterRecord>));

    public List<RosterRecord> Rosters
    {
        get { return (List<RosterRecord>)base.GetValue(RostersProperty); }
        set { base.SetValue(RostersProperty, value); }
    }

    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        if (propertyName == "Rosters")
        {
            if (Rosters != null)
            {
                try
                {
                    leftArrow.Source = "arrowactiveleft";
                    leftArrow.IsEnabled = true;
                    rightArrow.Source = "arrowactiveright";
                    rightArrow.IsEnabled = true;
                    monthName.Text = CurrentRosterDate.ToString("MMMM").ToUpper();
                    SetMonthDates(CurrentRosterDate);
                }
                catch (Exception ex)
                {
                    App.Current?.MainPage?.DisplayAlert("Error", $"{ex.Message}", "OK");
                }
            }
        }
    }

    void Handle_Tapped_Left(object sender, System.EventArgs e)
    {
        try
        {
            MoveLeft();
        }
        catch (Exception ex)
        {
            App.Current?.MainPage?.DisplayAlert("Error", $"{ex.Message}", "OK");
        }
    }

    void Handle_Tapped_Right(object sender, System.EventArgs e)
    {
        try
        {
            MoveRight();
        }
        catch (Exception ex)
        {
            App.Current?.MainPage?.DisplayAlert("Error", $"{ex.Message}", "OK");
        }
    }

    void OnSwiped(object sender, SwipedEventArgs e)
    {
        switch (e.Direction)
        {
            case SwipeDirection.Left:
                //MoveRight();
                break;
            case SwipeDirection.Right:
                //MoveLeft();
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
        if (leftArrow.IsEnabled != true) return;

        CurrentRosterDate = CurrentRosterDate.AddMonths(-1);
        bool canGoBack = _rosterViewPageViewModel.CanGoBackMonthly(CurrentRosterDate, _rosterViewPageViewModel.NumberOfWeeksAllowedToGoBack * 7);
        //bool canGoBack = RosterPageModel.CanGoBackMonthly(CurrentRosterDate, RosterPageModel.NumberOfWeeksAllowedToGoBack * 7);
        leftArrow.Source = "arrowinactiveleft";
        if (canGoBack)
        {
            leftArrow.Source = "arrowactiveleft";
        }
        leftArrow.IsEnabled = canGoBack;
        rightArrow.Source = "arrowactiveright";
        rightArrow.IsEnabled = true;
        monthName.Text = CurrentRosterDate.ToString("MMMM").ToUpper();
        SetMonthDates(CurrentRosterDate);
    }

    private void MoveRight()
    {
        if (rightArrow.IsEnabled != true) return;

        CurrentRosterDate = CurrentRosterDate.AddMonths(1);
        bool canGoForward = _rosterViewPageViewModel.CanGoForward(CurrentRosterDate, _rosterViewPageViewModel.NumberOfWeeksAllowedToGoForward * 7);
        leftArrow.Source = "arrowactiveleft";
        leftArrow.IsEnabled = true;
        rightArrow.Source = "arrowinactiveright";
        if (canGoForward)
        {
            rightArrow.Source = "arrowactiveright";
        }
        rightArrow.IsEnabled = canGoForward;
        monthName.Text = CurrentRosterDate.ToString("MMMM").ToUpper();
        SetMonthDates(CurrentRosterDate);
    }

    void SetMonthDates(DateTime currentDate)
    {
        var minmumDate = _rosterViewPageViewModel.GetDateCanGoBack(_rosterViewPageViewModel.NumberOfWeeksAllowedToGoBack * 7);
        var remainingdays = 7 - ((int)DateTime.Today.DayOfWeek + 1);
        var maximumdate = _rosterViewPageViewModel.GetDateCanGoForward(_rosterViewPageViewModel.NumberOfWeeksAllowedToGoForward * 7).AddDays(remainingdays);
        var stringDateParseFormats = new string[] { "d/M/yyyy", "dd/MM/yyyy" };

        var firstDate = _rosterViewPageViewModel.GetFirstOfMonth(currentDate);
        var lastDayOfMonth = DateTime.DaysInMonth(firstDate.Year, firstDate.Month);
        var lastDayOfMonthPre = DateTime.DaysInMonth(firstDate.AddMonths(-1).Year, firstDate.AddMonths(-1).Month);

        var dayOfWeek = (int)firstDate.DayOfWeek;
        int daysNumber = 0; // intended to represent Sunday
        int numberOfChildren = monthView.Children.Count;
        // 1. If you complicated code, you should comment it so other devs can get some context
        // 2. You shouldn't have complicated code
        // 3. If the code is complicated, refactor it once its working.
        for (int childIndex = 0; childIndex < numberOfChildren; ++childIndex)
        {
            var child = monthView.Children[childIndex];

            if (child is Label label)
            {
                //((Label)child).Text = "";
                //((Label)child).ClassId = null;

                label.Text = "";
                label.ClassId = null;

                var column = Grid.GetColumn(label);
                var row = Grid.GetRow(label);
                //var childs = monthView.Children.Where(r => Grid.GetRow(r) == row && Grid.GetColumn(r) == column).ToList();
                //var frame = childs[0] as Frame;
                //frame.BorderColor = Color.FromHex("EDEDED");
                //frame.BackgroundColor = Color.White;

                var frame = monthView.Children.OfType<Frame>().FirstOrDefault(r => Grid.GetRow(r) == row && Grid.GetColumn(r) == column);
                if (frame != null)
                {
                    frame.BorderColor = Color.FromArgb("#EDEDED");
                    frame.BackgroundColor = Color.FromArgb("#FFFFFF");

                    // This if statement handles any days for the previous month...
                    if (daysNumber < dayOfWeek)
                    {
                        var preDate = lastDayOfMonthPre - dayOfWeek + daysNumber + 1;

                        string preDateString = string.Format("{0}/{1}/{2}", preDate, firstDate.AddMonths(-1).Month, firstDate.AddMonths(-1).Year);

                        var preDatevalue = DateTime.ParseExact(preDateString, stringDateParseFormats, CultureInfo.InvariantCulture, DateTimeStyles.None);
                        //((Label)child).Text = preDate.ToString();
                        label.Text = preDate.ToString();

                        if (preDatevalue.Date >= minmumDate.Date && preDatevalue.Date <= maximumdate.Date)
                            ((Label)child).ClassId = preDatevalue.ToString();

                        var rosterDay = Rosters.FirstOrDefault(d => d.WorkdayDate.Date == preDatevalue.Date);

                        ProcessDate(label, frame, rosterDay);

                        HandleSpecialDays(label, frame, preDatevalue, rosterDay);
                    }

                    // This if statement handles any days for the current month...
                    if (daysNumber >= dayOfWeek && firstDate.Day <= lastDayOfMonth && (firstDate.Month <= maximumdate.Month || firstDate.Year <= maximumdate.Year))
                    {
                        var rosterDay = Rosters.FirstOrDefault(d => d.WorkdayDate.Date == firstDate.Date);
                        ProcessDate(label, frame, rosterDay);

                       label.Text = firstDate.Day.ToString();

                        if (firstDate.Date >= minmumDate.Date && firstDate.Date <= maximumdate.Date)
                            label.ClassId = firstDate.ToString();

                        HandleSpecialDays(label, frame, firstDate, rosterDay);

                        firstDate = firstDate.AddDays(1);
                    }
                    daysNumber++;
                }
            }
                
        }
    }

    private static void ProcessDate(View child, Frame frame, RosterRecord? rosterDay)
    {
        // If it's not a roster day - set the text color to faded gray indicating that it's not a work/roster day
        if (rosterDay == null)
        {
            ((Label)child).TextColor = new Color(0, 0, 0, 0.2f  );
            return;
        }

        // From here - we are processing a rosterDay...
        ((Label)child).TextColor = Colors.Black; // default font text to be black indicating this is a day their to be at work

        // frame.HasShadow = true;
        // If we have a roster day - check if it's CDO - mark as green. Otherwise all others should be black on white background to indicate a work/roster day
        if (!string.IsNullOrEmpty(rosterDay.DutyID) && rosterDay.DutyID.Contains("CDO", StringComparison.OrdinalIgnoreCase))
        {
            ((Label)child).TextColor = Colors.White;
            frame.BorderColor = Color.FromArgb("#007B4B");
            frame.BackgroundColor = Color.FromArgb("#78BE20");
        }
        else if (!string.IsNullOrEmpty(rosterDay.DutyID)) // Handle days off/absentee/away from work days
        {
            if (rosterDay.DutyID.Contains("OFF", StringComparison.OrdinalIgnoreCase))
            {
                ((Label)child).TextColor = new Color(0, 0, 0, 0.2f);
            }
            else if (rosterDay.DutyID.Contains("ABS", StringComparison.OrdinalIgnoreCase))
            {
                ((Label)child).TextColor = new Color(0, 0, 0, 0.2f);
            }
            else if (rosterDay.DutyID.Contains("DNC", StringComparison.OrdinalIgnoreCase))
            {
                ((Label)child).TextColor = new Color(0, 0, 0, 0.2f);
            }
            else if (rosterDay.DutyID.Contains("NOTR", StringComparison.OrdinalIgnoreCase))
            {
                ((Label)child).TextColor = new Color(0, 0, 0, 0.2f);
            }
            else if (rosterDay.DutyID.Contains("PLD", StringComparison.OrdinalIgnoreCase))
            {
                ((Label)child).TextColor = new Color(0, 0, 0, 0.2f);
            }
        }
    }

    private static void HandleSpecialDays(View child, Frame frame, DateTime dateVal, RosterRecord? rosterDay)
    {
        // Hande todays date:
        if (dateVal.Date == DateTime.Today.Date)
        {
            ((Label)child).TextColor = Color.FromArgb("#CF4520");
            frame.BorderColor = Color.FromArgb("#CF4520");
            frame.BackgroundColor = Color.FromArgb("#1ACF4520");
        }
    }
}