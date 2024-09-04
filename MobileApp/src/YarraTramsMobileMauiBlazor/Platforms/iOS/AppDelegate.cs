using Core;
using Data.SharePoint.Authentication;
using Foundation;
using Microsoft.Identity.Client;
using UIKit;
using UserNotifications;

namespace YarraTramsMobileMauiBlazor
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        private static string RedirectUrl()
        {
            string AppId = "au.com.yarratrams.dingyarratrams";
            return $"msauth.{AppId}://auth";
        }

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            PlatformConfig.Instance.RedirectUri = RedirectUrl();
            PlatformConfig.Instance.ParentWindow = null;
            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            AuthenticationContinuationHelper.SetAuthenticationContinuationEventArgs(url);
            return true;
        }

        // Security/Jailbreak check:
        //private void SetUpSecurity()
        //{
        //    if (Cryoprison.Factory.IsSupported)
        //    {
        //        var env = Cryoprison.Factory.CreateEnvironment();

        //        env.Reporter.OnJailbreakReported = (id) =>
        //        {
        //            Console.WriteLine($"Jailbreak: {id ?? "<null>"}");
        //        };

        //        env.Reporter.OnExceptionReported = (message, exception) =>
        //        {
        //            Console.WriteLine($"Jailbreak Error: {message}");
        //            Console.WriteLine(exception.ToString());
        //        };

        //        jailbreakDetector = Cryoprison.Factory.CreateJailbreakDetector(env, simulatorFriendly: false);

        //        App.IsJailBroken = () =>
        //        {
        //            return CustomIsJailBroken();
        //        };

        //        App.JailBreaks = () =>
        //        {
        //            return this.jailbreakDetector.Violations;
        //        };
        //    }
        //    else
        //    {
        //        App.IsJailBroken = () =>
        //        {
        //            return false;
        //        };

        //        App.JailBreaks = () =>
        //        {
        //            return new string[0];
        //        };
        //    }
        //}

        //private bool CustomIsJailBroken()
        //{
        //    bool isJailBroken = jailbreakDetector.IsJailbroken;
        //    if (!isJailBroken) return false;
        //    var violations = jailbreakDetector.Violations;
        //    if (violations == null) return false;

        //    // return true IF there is only one violation and it is 'EMBEDDED_MOBILEPROVISION_SHOULD_BE_PRESENT' - as this is valid for app store apps...
        //    var violationsList = violations.ToList();
        //    if (violationsList.Count == 1 && violationsList[0] == "EMBEDDED_MOBILEPROVISION_SHOULD_BE_PRESENT") return false;

        //    // otherwise:
        //    if (isJailBroken)
        //    {
        //        return true;
        //    }
        //    return false;
        //}
    }
}
