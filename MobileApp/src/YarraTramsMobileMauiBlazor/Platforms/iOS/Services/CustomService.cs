using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTramsMobileMauiBlazor.Interfaces;

namespace YarraTramsMobileMauiBlazor.Platforms.Services
{
    public class CustomService : ICustomService
    {
        public void CloseApplication()
        {
            if (Application.Current?.MainPage != null)
            {
                Application.Current.MainPage.DisplayAlert(
                "Close App",
                "Please close the app using the app switcher.",
                "OK");
            }
            else
            {
                Console.WriteLine("\"MainPage is null. Unable to display error message to the user.\"");
            }

        }

        public void UpdateBadgeCount()
        {
            //var badgeNumber = YarraTramsMobileMauiBlazor.Helpers.Utils.getnew
        }
    }
}
