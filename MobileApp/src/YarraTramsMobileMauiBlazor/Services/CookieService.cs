#if ANDROID
using Android.OS;
using Android.Webkit;
#elif IOS
using Foundation;
#endif
using Core.Interfaces;

namespace YarraTramsMobileMauiBlazor.Services
{
    public class CookieService : ICookieService
    {
        [Obsolete]
        public void ClearAllCookies()
        {
#if ANDROID
            var cookieManager = CookieManager.Instance;
            if (Build.VERSION.SdkInt >= BuildVersionCodes.LollipopMr1)
            {
                cookieManager?.RemoveAllCookies(null);
                cookieManager?.Flush();
            }
            else
            {
                // Fallback for older versions
                cookieManager.RemoveAllCookie();
            }
#elif IOS
            NSHttpCookie[] cookies = NSHttpCookieStorage.SharedStorage.Cookies;
            foreach (var cookie in cookies)
            {
                NSHttpCookieStorage.SharedStorage.DeleteCookie(cookie);
            }
#endif
        }

        public bool HasCookies()
        {
#if ANDROID
            var cookieManager = CookieManager.Instance;
            return cookieManager.HasCookies;
#elif IOS
            var cookieStorage = NSHttpCookieStorage.SharedStorage;
            return cookieStorage.Cookies.Any();
#else
            throw new NotImplementedException();
#endif
        }
    }
}
