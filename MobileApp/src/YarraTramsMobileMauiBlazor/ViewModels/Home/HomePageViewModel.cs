using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTramsMobileMauiBlazor.Interfaces;

namespace YarraTramsMobileMauiBlazor.ViewModels.Home
{
    public class HomePageViewModel : ViewModelBase
    {
        public HomePageViewModel(INavigationService navigationService) : base(navigationService)
        {
        }
    }
}
