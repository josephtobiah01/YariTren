using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Core.Models
{
    public class RosterConfirmation : SpItem
    {
        public DateTime WorkdayDate { get; set; }
        public string Depot { get; set; }
        public string DepotWeekIndex { get; set; }
        public bool RosterConfirmed { get; set; }
        
        public bool IsToday
        {
            get
            {
                return WorkdayDate.Date == DateTime.Today.Date;
            }
        }
    }
}
