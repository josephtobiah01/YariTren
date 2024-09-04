using Core.Models;
using SharedLogic.Roster;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IRosterService
    {
        event EventHandler<string> RosterLoadFailed;
        event EventHandler<RosterLoadedEventArgs> RosterLoaded;

        Task<List<RosterRecord>> Load(UserProfile userProfile, bool someFlag);
        Task<List<RosterRecord>> GetConfirmedRosterData(UserProfile userProfile);
        bool HasRoster(UserProfile userProfile);
        //Task Load(UserProfile userProfile, bool someFlag);
    }
}
