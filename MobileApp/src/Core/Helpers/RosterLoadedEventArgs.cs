using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLogic.Roster
{
    public class RosterLoadedEventArgs : EventArgs
    {
        public List<RosterRecord> Records { get; }

        public RosterLoadedEventArgs(List<RosterRecord> records)
        {
            Records = records ?? throw new ArgumentNullException(nameof(records));
        }

    }

    public class RosterLoadFailedEventArgs : EventArgs
    {
        public Exception Exception { get; }
        public string Reason { get; }
        public RosterLoadFailedEventArgs(
            Exception exception,
            string reason)
        {
            Exception = exception;
            Reason = reason;
        }

        


    }
}
