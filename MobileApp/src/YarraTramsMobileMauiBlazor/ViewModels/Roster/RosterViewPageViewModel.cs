using Core;
using Core.Exceptions;
using Core.Interfaces;
using Core.Models;
using SharedLogic.Roster;
using System.Collections.ObjectModel;
using System.Globalization;
using YarraTramsMobileMauiBlazor.Interfaces;
using YarraTramsMobileMauiBlazor.Views.Login;

namespace YarraTramsMobileMauiBlazor.ViewModels.Roster
{
    public enum CalendarState { Monthly, Weekly }
    public class RosterViewPageViewModel : ViewModelBase
    {
        #region Fields

        private string? _weekRange;
        private string? _dutyID;
        private string? _dutyType;
        private string? _locationStart;
        private string? _startTime;
        private string? _endTime;
        private string? _mealLocation;
        private string? _mealStart;
        private string? _mealEnd;
        private string? _workingDuration;
        private string? _splitDuration;
        private string? _rosterDetailsPopupTitle;
        private string? _rosterSimplePopupTitle;


        private bool _isMonthlyVisible;
        private bool _isWeeklyVisible;
        private bool _isCalendarVisible;
        private bool _isWeekVisible;
        private bool _isRosterSimplePopupOpen = false;
        private bool _isRosterDetailsPopupOpen = false;
        private bool _isWeekLeftArrow;
        private bool _isWeekRightArrow;
        private bool _isOpen;

        private DateTime _weekStartDate;
        private DateTime _weekEndDate;
        private DateTime _rosterDate;
        private DateTime _currentRosterDate = DateTime.Now;


        private List<RosterRecord>? _rosters;
        private ObservableCollection<RosterRecord>? _weekListItems;

        private readonly IClientManager _clientManager;
        private IRosterService _rosterService;
        private readonly INavigationService _navigationService;
        private readonly IUserProfileService _userProfileService;


        #endregion Fields

        #region Properties

        #region String Properties

        public string? WeekRange 
        {
            get { return _weekRange; }
            set { SetPropertyValue(ref _weekRange, value); }
        }

        public string? DutyID
        {
            get { return _dutyID; }
            set { SetPropertyValue(ref _dutyID, value); }
        }

        public string? DutyType
        {
            get { return _dutyType; }
            set { SetPropertyValue(ref _dutyType, value); }
        }

        public string? LocationStart
        {
            get { return _locationStart; }
            set { SetPropertyValue(ref _locationStart, value); }
        }

        public string? StartTime
        {
            get { return _startTime; }
            set { SetPropertyValue(ref _startTime, value); }
        }

        public string? EndTime
        {
            get { return _endTime; }
            set { SetPropertyValue(ref _endTime, value); }
        }

        public string? MealLocation
        {
            get { return _mealLocation; }
            set { SetPropertyValue(ref _mealLocation, value); }
        }

        public string? MealStart
        {
            get { return _mealStart; }
            set { SetPropertyValue(ref _mealStart, value); }
        }

        public string? MealEnd
        {
            get { return _mealEnd; }
            set { SetPropertyValue(ref _mealEnd, value); }
        }

        public string? WorkingDuration
        {
            get { return _workingDuration; }
            set { SetPropertyValue(ref _workingDuration, value); }
        }

        public string? SplitDuration
        {
            get { return _splitDuration; }
            set { SetPropertyValue(ref _splitDuration, value); }
        }

        public string? RosterDetailsPopupTitle
        {
            get { return _rosterDetailsPopupTitle; }
            set { SetPropertyValue(ref _rosterDetailsPopupTitle, value); }
        }

        public string? RosterSimplePopupTitle
        {
            get { return _rosterSimplePopupTitle; }
            set { SetPropertyValue(ref _rosterSimplePopupTitle, value); }
        }

        #endregion String Properties

        #region Boolean Properties

        public bool IsOpen
        {
            get { return _isOpen; }
            set { SetPropertyValue(ref _isOpen, value); }
        }

        public bool IsWeekLeftArrow
        {
            get { return _isWeekLeftArrow; }
            set { SetPropertyValue(ref _isWeekLeftArrow, value); }
        }

        public bool IsWeekRightArrow
        {
            get { return _isWeekRightArrow; }
            set { SetPropertyValue(ref _isWeekRightArrow, value); }
        }

        public bool IsRosterDetailsPopupOpen
        {
            get { return _isRosterDetailsPopupOpen; }
            set { SetPropertyValue(ref _isRosterDetailsPopupOpen, value); }
        }

        public bool IsRosterSimplePopupOpen
        {
            get { return _isRosterSimplePopupOpen; }
            set { SetPropertyValue(ref _isRosterSimplePopupOpen, value); }
        }

        public bool IsCalendarVisible
        {
            get { return _isCalendarVisible; }
            set { SetPropertyValue(ref _isCalendarVisible, value); }
        }
        public bool IsWeekVisible
        {
            get { return _isWeekVisible; }
            set { SetPropertyValue(ref _isWeekVisible, value); }
        }
        public bool IsMonthlyVisible
        {
            get { return _isMonthlyVisible; }
            set { SetPropertyValue(ref _isMonthlyVisible, value); }
        }
        public bool IsWeeklyVisible
        {
            get { return _isWeeklyVisible; }
            set { SetPropertyValue(ref _isWeeklyVisible, value); }
        }

        #endregion Boolean Properties

        #region Collection Properties


        public ObservableCollection<RosterRecord>? WeekListItems
        {
            get { return _weekListItems; }
            set { SetPropertyValue(ref _weekListItems, value); }
        }

        public List<RosterRecord>? Rosters
        {
            get { return _rosters; }
            set { SetPropertyValue(ref _rosters, value); }
        }


        #endregion Collection Properties

        #region DateTime

        public DateTime WeekStartDate
        {
            get { return _weekStartDate; }
            set { SetPropertyValue(ref _weekStartDate, value); }
        }

        public DateTime WeekEndDate
        {
            get { return _weekEndDate; }
            set { SetPropertyValue(ref _weekEndDate, value); }
        }

        public DateTime RosterDate
        {
            get { return _rosterDate; }
            set { SetPropertyValue(ref _rosterDate, value); }
        }

        public DateTime CurrentRosterDate
        {
            get { return _currentRosterDate; }
            set { SetPropertyValue(ref _currentRosterDate, value); }
        }

        #endregion DateTime

        #region Command Properties

        public Command WeekLeftArrow { get; set; }
        public Command WeekRightArrow { get; set; }
        public Command ToggleButton { get; set; }
        //public Command DateClicked { get; set; }

        #endregion Command Properties

        public int NumberOfWeeksAllowedToGoBack = 4;

        public int NumberOfWeeksAllowedToGoForward = 4;

        #endregion Properties

        public RosterViewPageViewModel(
            INavigationService navigationService,
            IRosterService rosterService,
            IUserProfileService userProfileService,
            IClientManager clientManager) : base(navigationService)
        {
            _rosterService = rosterService;
            _navigationService = navigationService;
            _userProfileService = userProfileService;
            _clientManager = clientManager;

            Initialize();

            _rosterService.RosterLoaded += RosterService_RosterLoaded;

            WeekListItems = new ObservableCollection<RosterRecord>();
            Rosters = new List<RosterRecord>();

            ToggleButton = new Command(ToggleButtonCommand);
            //DateClicked = new Command(DateClickedCommand);
            WeekLeftArrow = new Command(WeekLeftArrowCommand);
            WeekRightArrow = new Command(WeekRightArrowCommand);

        }

        

        public void OnPageLoaded()
        {
            // Ensure DefaultRequestHeaders are set
            //_clientManager.InitializeClient();
        }

        private void RosterService_RosterLoaded(object? sender, RosterLoadedEventArgs e)
        {
            if (e != null)
            {
                Rosters = e.Records;
            }
            SetSelectedDateWeekView();
        }

        public async void Initialize()
        {
            List<RosterRecord> records = new List<RosterRecord>();
            //RosterDetailsPopupSwiped = RosterDetailsPopupSwipedCommand;
            //RosterSimplePopupSwiped = RosterSimplePopupSwipedCommand;

            try
            {
                SetCalendarState(CalendarState.Weekly);
                //ShowAlert("Loading...");
                DateTime date = DateTime.Now;
                var userProfile = await _userProfileService.GetCurrentUserProfile();
                records = await _rosterService.Load(userProfile, true);
                InitializeProperties(records, date);
            }
            catch (Exception ex)
            {
                ShowAlert($"Error:{ex.Message}", "The roster data is not available. Refer to Yardsheet.");
                //Utils.HandleError(ex, "The roster data is not available. Refer to Yardsheet.");
                //_dialogs.HideLoading();
                //await ((MasterNavigationService)Application.Current.MainPage).PopPage(false, true);
            }

           //SetSelectedDateWeekView();

        }

        public void InitializeProperties(List<RosterRecord> records, DateTime selectedDate)
        {
            if (records == null || records.Count == 0)
            {
                // Handle the case when the records is empty or null
                ShowAlert("Warning", "No roster data available.");
                return;
            }
            SetProperties(records[0]);

            //RosterDetailsPopupTitle = selectedDate.ToString("MMMM dd, yyyy");
            //RosterSimplePopupTitle = selectedDate.ToString("MMMM dd, yyyy");
        }

        private void SetProperties(RosterRecord record) 
        {
            if (record == null) return;
            DutyID = record.DutyID;
            DutyType = record.DutyType;
            LocationStart = record.LocationStart;
            StartTime = record.StartTime;
            EndTime = record.EndTime;
            MealLocation = record.MealLocation;
            MealStart = record.MealStart;
            MealEnd = record.MealEnd;
            WorkingDuration = record.WorkingDuration;
            SplitDuration = record.SplitDuration;
        }

        private void WeekLeftArrowCommand()
        {
            IsOpen = true;
            try
            {
                
                CurrentRosterDate = CurrentRosterDate.AddDays(-7);
                SetSelectedDateWeekView();
            }
            catch (Exception ex)
            {
                ShowAlert("Error", $"{ex.Message}");
            }
            finally
            {
                IsOpen = false;
            }
        }

        private void WeekRightArrowCommand()
        {
            IsOpen= true;
            try
            {
                CurrentRosterDate = CurrentRosterDate.AddDays(7);
                SetSelectedDateWeekView();
            }
            catch (Exception ex)
            {
                ShowAlert("Error", $"{ex.Message}");
            }
            finally
            {
                IsOpen = false; 
            }
        }

        private void ToggleButtonCommand()
        {
            try
            {
                if (IsMonthlyVisible)
                {
                    SetCalendarState(CalendarState.Weekly);
                }
                else
                {
                    SetCalendarState(CalendarState.Monthly);
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error", $"{ex.Message}");
            }
        }

        //private void RosterSimplePopupSwipedCommand(object? sender, SwipedEventArgs e)
        //{
        //    if (e.Direction == SwipeDirection.Left || e.Direction == SwipeDirection.Right)
        //    {
        //        IsRosterSimplePopupOpen = false;
        //    }
        //}

        //private void RosterDetailsPopupSwipedCommand(object? sender, SwipedEventArgs e)
        //{
        //    if (e.Direction == SwipeDirection.Left || e.Direction == SwipeDirection.Right)
        //    {
        //        IsRosterDetailsPopupOpen = false;
        //    }
        //}


       
        private async void RosterProviderOnRosterDataError(Exception ex, string reason)
        {
            Rosters = new List<RosterRecord>();
            SetSelectedDateWeekView();
            //_dialogs.HideLoading();

            if (ex.GetBaseException().GetType() == typeof(InvalidUsernameOrPasswordException))
            {
                ShowConfirmation("Account issue", $"{Consts.InvalidUserNameOrPasswordMessage}", _ => { }, "OK" );
                //await GetConfirmation("Account issue", Consts.InvalidUserNameOrPasswordMessage + " from the sign in page");// wait for them to acknowledge...
                await NavigateToLoginPage();
                return;
            }
            if (ex.GetBaseException().GetType() == typeof(AccountDisabledException))
            {
                ShowConfirmation("Account locked", $"{Consts.LockedAccountMessage}", _ => { }, "OK");
                //await GetConfirmation("Account locked", Consts.LockedAccountMessage);  // wait for them to acknowledge...
                await NavigateToLoginPage();
                return;
            }
            //Utils.HandleError(ex, "The Roster data could not be accessed.  Please refer to the Yard Sheet.");
            ShowAlert($"{ex.Message}", "The Roster data could not be accessed. Please refer to the Yard Sheet.");
        }

        //private async Task GetConfirmation(string title, string message)
        //{
        //    var result = await _dialogs.ConfirmAsync(new ConfirmConfig
        //    {
        //        Message = message,
        //        OkText = "OK",
        //        Title = title,
        //        CancelText = string.Empty
        //    });
        //}

        private async Task NavigateToLoginPage()
        {
            await _navigationService.NavigateTo(nameof(LoginPage));
        }


        //protected override void ViewIsAppearing(object sender, EventArgs e)
        //{
        //    base.ViewIsAppearing(sender, e);
        //}

        //protected virtual void 
        //{
        //    base.ViewIsDisappearing(sender, e);
        //    _rosterProvider.RosterLoaded -= RosterProviderOnRosterLoaded;
        //    _rosterProvider.RosterDataError -= RosterProviderOnRosterDataError;
        //}

        //public override Task OnNavigatedFrom(bool isForwardNavigation)
        //{
        //    return base.OnNavigatedFrom(isForwardNavigation);

        //    _rosterService.RosterLoaded -= OnRosterLoaded;

        //}

        //private void OnRosterLoaded(object? sender, RosterLoadedEventArgs e)
        //{
        //    RosterProviderOnRosterLoaded(e.Records);
        //}
        //public void OnDisappearing()
        //{
        //    _rosterService.RosterLoaded -= OnRosterLoaded;
        //}

        

        public void SetSelectedDateWeekView()
        {
                try
                {
                    WeekStartDate = GetSundayStartOfWeek(CurrentRosterDate);
                    WeekEndDate = WeekStartDate.AddDays(7).AddSeconds(-1);
                    WeekRange = string.Format("{0} {1} - {2} {3}", WeekStartDate.ToString("dd"), WeekStartDate.ToString("MMM").ToUpper(), WeekEndDate.ToString("dd"), WeekEndDate.ToString("MMM").ToUpper());
                    if (WeekRange.Contains("."))
                    {
                        WeekRange = WeekRange.Replace(".", string.Empty);
                    }
                    SetWeekViewDates();
                    SetLeftArrowWeekView();
                    SetRightArrowWeekView();
                }
                catch (Exception ex)
                {
                     ShowAlert("Error", $"{ex.Message}");
                    //Utils.HandleError(ex);
                }
        }

        public void SetLeftArrowWeekView()
        {
            try
            {
                IsWeekLeftArrow = CanGoBackWeekly(CurrentRosterDate, NumberOfWeeksAllowedToGoBack * 7);
            }
            catch (Exception ex)
            {
                ShowAlert("Error", $"{ex.Message}");
            }
        }

        void SetRightArrowWeekView()
        {
            try
            {
                IsWeekRightArrow = CanGoForward(CurrentRosterDate, NumberOfWeeksAllowedToGoForward * 7);
            }
            catch (Exception ex)
            {
                ShowAlert("Error", $"{ex.Message}");
            }
        }

        void SetWeekViewDates()
        {
            if (WeekListItems == null)
            {
                WeekListItems = new ObservableCollection<RosterRecord>();
            }
            else
            {
                WeekListItems.Clear();
            }

            for (int i = 0; i < 7; i++)
            {
                var rosterRecord = Rosters?.FirstOrDefault(d => d.WorkdayDate.Date == WeekStartDate.AddDays(i).Date);
                if (rosterRecord != null)
                {
                    rosterRecord.IsNonRoster = false;
                    WeekListItems.Add(rosterRecord);
                }
                else
                {
                    var notRosterDate = new RosterRecord
                    {
                        WorkdayDate = WeekStartDate.AddDays(i),
                        IsNonRoster = true
                    };
                    WeekListItems.Add(notRosterDate);
                }
            }

            for (int i = 0; i < WeekListItems.Count; i++)
            {
                WeekListItems[i].Index = i;
            }
        }

        void SetCalendarState(CalendarState state)
        {
            switch (state)
            {
                case CalendarState.Monthly:
                    IsMonthlyVisible = true;
                    IsWeeklyVisible = false;
                    IsCalendarVisible = true;
                    IsWeekVisible = false;
                    break;
                case CalendarState.Weekly:
                    IsMonthlyVisible = false;
                    IsWeeklyVisible = true;
                    IsCalendarVisible = false;
                    IsWeekVisible = true;
                    break;
                default:
                    break;
            }
        }

        public Command DateClicked => new Command((object parameter) =>
        {
            try
            {
                var label = parameter as Label;
                if (label?.ClassId != null)
                {
                    var selectedDate = DateTime.Parse(label.ClassId);
                    var item = Rosters?.FirstOrDefault(d => d.WorkdayDate.Date == selectedDate.Date);
                    if (item != null)
                    {
                        IsRosterDetailsPopupOpen = true;
                        SetProperties(item);
                        RosterDetailsPopupTitle = selectedDate.ToString("MMMM dd, yyyy");
                    }
                    else
                    {
                        IsRosterSimplePopupOpen = true;
                        RosterSimplePopupTitle = selectedDate.ToString("MMMM dd, yyyy");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowAlert("Error", $"{ex.Message}");
            }
        });

        



        //public Command NewsTab
        //{
        //    get
        //    {
        //        return new Command(async () =>
        //        {
        //            try
        //            {

        //                await ((MasterNavigationService)Application.Current.MainPage).PopPage(false, true);
        //            }
        //            catch (Exception ex)
        //            {
        //                ShowAlert("Error", $"{ex.Message}");
        //            }
        //        });
        //    }
        //}

        //public Command MyToolsTab
        //{
        //    get
        //    {
        //        return new Command(async () =>
        //        {
        //            try
        //            {
        //                await ((MasterNavigationService)Application.Current.MainPage).PopPage(false, true);
        //            }
        //            catch (Exception ex)
        //            {
        //                ShowAlert("Error", $"{ex.Message}");
        //            }
        //        });
        //    }
        //}

        /// <summary>
        /// The following method takes number of days we are allowed back. From this date it return the preceding Sunday as the start of that week as the date that is returned.
        /// </summary>
        /// <param name="daysAllowedBack"></param>
        /// <returns></returns>
        internal DateTime GetDateCanGoBack(int daysAllowedBack)
        {
            var dateBack = DateTime.Now.AddDays(-daysAllowedBack);
            return GetSundayStartOfWeek(dateBack);
        }

        internal DateTime GetDateCanGoForward(int daysAllowedForward)
        {
            return DateTime.Now.AddDays(daysAllowedForward);
        }

        internal bool CanGoBackWeekly(DateTime dateToCheck, int daysAllowedBack)
        {
            var dateAllowedToGoBack = GetDateCanGoBack(daysAllowedBack);
            return (dateAllowedToGoBack.Date < GetSundayStartOfWeek(dateToCheck).Date);
        }

        internal bool CanGoBackMonthly(DateTime dateToCheck, int daysAllowedBack)
        {
            var dateAllowedToGoBack = GetDateCanGoBack(daysAllowedBack);
            return (dateAllowedToGoBack.Date < GetFirstOfMonth(dateToCheck).Date);
        } 


        internal bool CanGoForward(DateTime dateToCheck, int daysAllowedForward)
        {
            var dateAllowedToGoFoward = GetDateCanGoForward(daysAllowedForward);
            return (dateToCheck.Date < dateAllowedToGoFoward.Date);
        }

        internal static DateTime GetSundayStartOfWeek(DateTime weekDate)
        {
            return weekDate.AddDays(-((int)weekDate.DayOfWeek)); // Gives us the previous Sunday...
        }

        internal DateTime GetFirstOfMonth(DateTime monthDate)
        {
            var month = monthDate.Month.ToString();
            if (monthDate.Month < 10)
            {
                month = string.Format("0{0}", monthDate.Month);
            }

            var firstOfMonthString = "01/" + month + "/" + monthDate.Year;
            return DateTime.ParseExact(firstOfMonthString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
    }
}
