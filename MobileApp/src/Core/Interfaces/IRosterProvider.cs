using Core.Models;
using SharedLogic.Roster;
using System;

namespace Core.Interfaces
{
    /// <summary>
    /// Loads roster data async and supplies data through event system.
    /// </summary>
    public interface IRosterProvider
    {
        //event RosterProvider.RosterLoadedDelegate RosterLoaded;
        //event RosterProvider.RosterLoadFailedDelegate RosterDataError;

        event EventHandler<RosterLoadedEventArgs> RosterLoaded;
        event EventHandler<RosterLoadFailedEventArgs> RosterLoadFailed;
        bool HasRoster(UserProfile userProfile);
        bool InProgress { get; set; }
        void Load(UserProfile userProfile, bool reload = false);
    }
}