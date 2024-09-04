using System;
using System.Collections.Generic;
using Core.Models;

namespace SharedLogic
{
    public static class RosterManager
    {
        public static List<RosterRecord> GetRoster()
        {
            List<RosterRecord> items = new List<RosterRecord>();
            for (var i = 0; i < 14; i++)
            {
                items.Add(new RosterRecord()
                {
                    WorkdayDate = new DateTime(2018, 11,15 + i, 0, 0, 0),
                    EmployeeID = "12345",
                    EmployeeName = "Test Employee",
                    LocationStart = "sb06",
                    DutyID = "A/L",
                    DutyType = "E",
                    StartTime = "3:15",
                    EndTime = "12:31",
                    MealStart = "7:31",
                    MealEnd = "8:22",
                    MealLocation = "sb06",
                    Depot = "SBANK",
                    WorkingDuration = "0h00",
                    SplitDuration = "0h00"
                });
            }
            items[8].IsCDO = true;
            return items;
        }

    }
}
