using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTramsMobileMauiBlazor.Interfaces;

namespace YarraTramsMobileMauiBlazor.ViewModels.Roster
{
    public class RosterSimplePopupViewModel : ViewModelBase
    {
        private bool _isPopupOpen;
        public bool IsPopupOpen
        {
            get { return _isPopupOpen; }
            set { SetPropertyValue(ref _isPopupOpen, value); }
        }

        public EventHandler<SwipedEventArgs> Swiped { get; set; }
        public RosterSimplePopupViewModel(INavigationService navigationService) : base(navigationService)
        {
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
