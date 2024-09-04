using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Core.Helpers;

namespace Core.Models
{
    public class RosterRecord : INotifyPropertyChanged
    {
        public DateTime WorkdayDate { get; set; }
        public int Id { get; set; }
        public bool IsOpen { get; set; }
        public string Depot { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public string DutyID { get; set; }
        public string DutyType { get; set; }
        public string StartTime { get; set; }
        public string MealLocation { get; set; }
        public string MealStart { get; set; }
        public string MealEnd { get; set; }
        public string EndTime { get; set; }
        public string WorkingDuration { get; set; }
        public string SplitDuration { get; set; }
        public string LocationStart { get; set; }
        public bool IsNonRoster { get; set; }

        public int Index { get; set; }

        public string DepotWeekIndex { get; set; }

        public bool IsToday
        {
            get
            {
                return WorkdayDate.Date == DateTime.Today.Date;
            }
        }

        private bool _isExtended;
        /// <summary>
        /// This property is for expanding the week view cell.  When its true we drop down the day details. 
        /// </summary>
        public bool IsExtended
        {

            set
            {
                if (_isExtended != value)
                {
                    _isExtended = value;
                    OnPropertyChanged("IsExtended");

                }
            }
            get
            {
                return _isExtended;
            }
        }

        public string WeekOfDate
        {
            get
            {
                var strDate = WorkdayDate.ToString("ddd").ToUpper();
                if (strDate.Contains("."))
                {
                    strDate = strDate.Replace(".", string.Empty);
                }
                return strDate;
            }
        }

        public string WorkDayDateDayString
        {
            get
            {
                var strDate = WorkdayDate.ToString("dd").ToUpper();
                if (strDate.Contains("."))
                {
                    strDate = strDate.Replace(".", string.Empty);
                }
                return strDate;
            }
        }

        // review hastus script...
        private string hashValue = null;
        public string HashValue
        {
            get
            {
                if (hashValue != null) return hashValue;
                // else set and return it...
                return hashValue;
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
