using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTramsMobileMauiBlazor.Interfaces
{
    public interface IRosterProvider
    {
        event EventHandler<List<RosterRecord>> RosterLoaded;
        event EventHandler<string> RosterLoadFailed; // Using string to pass error message
        Task LoadRosterAsync(UserProfile userProfile, bool reload = false);
        bool HasRoster(UserProfile userProfile);
        bool InProgress { get; set; }
    }
}
