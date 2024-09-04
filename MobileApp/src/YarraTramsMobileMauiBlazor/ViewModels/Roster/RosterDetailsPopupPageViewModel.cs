using Core.Models;
using DevExpress.Xpo.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using YarraTramsMobileMauiBlazor.Interfaces;

namespace YarraTramsMobileMauiBlazor.ViewModels.Roster
{
    public class RosterDetailsPopupPageViewModel : ViewModelBase
    {
        private bool _isPopupOpen;
        public bool IsPopupOpen
        {
            get { return _isPopupOpen; }
            set { SetPropertyValue(ref _isPopupOpen, value); }
        }

        public string DutyID { get; set; }
        public string DutyType { get; set; }
        public string LocationStart { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string MealLocation { get; set; }
        public string MealStart { get; set; }
        public string MealEnd { get; set; }
        public string WorkingDuration { get; set; }
        public string SplitDuration { get; set; }

        public string PopupTitle { get; set; }

        public EventHandler<SwipedEventArgs> Swiped { get; set; }
        public RosterDetailsPopupPageViewModel(
            INavigationService navigationService,
            RosterRecord model, 
            DateTime selectedDate) : base(navigationService)
        {
            DutyID = model.DutyID;
            DutyType = model.DutyType;
            LocationStart = model.LocationStart;
            StartTime = model.StartTime;
            EndTime = model.EndTime;
            MealLocation = model.MealLocation;
            MealStart = model.MealStart;
            MealEnd = model.MealEnd;
            WorkingDuration = model.WorkingDuration;
            SplitDuration = model.SplitDuration;

            PopupTitle = selectedDate.ToString("MMMM dd, yyyy");

            IsPopupOpen = false;

            Swiped = SwipedCommand;
        }

        private void SwipedCommand(object? sender, SwipedEventArgs args)
        {
            if (args.Direction == SwipeDirection.Left || args.Direction == SwipeDirection.Right)
            {
                IsPopupOpen = false;
            }
        }


       

        
    }
}
