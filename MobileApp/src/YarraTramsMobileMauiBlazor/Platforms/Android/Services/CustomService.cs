using Core.Interfaces;
using YarraTramsMobileMauiBlazor.Interfaces;

namespace YarraTramsMobileMauiBlazor.Platforms.Services
{
    public class CustomService : ICustomService
    {
        public void CloseApplication()
        {
            var activity = Platform.CurrentActivity;

            if (activity != null)
            {
                activity.FinishAffinity();
            }
            else
            {
                Console.WriteLine("Activity is null. Unable to close application.");
            }
        }

        public void UpdateBadgeCount()
        {
            return;
        }
    }
}
