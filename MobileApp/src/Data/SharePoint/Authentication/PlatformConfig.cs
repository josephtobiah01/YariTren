using Core;
using Microsoft.Maui.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.SharePoint.Authentication
{
    public class PlatformConfig
    {
        /// <summary>
        /// Instance to store data
        /// </summary>
        public static PlatformConfig Instance { get; } = new PlatformConfig();

        /// <summary>
        /// Platform specific Redirect URI
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Platform specific parent window
        /// </summary>
        public object ParentWindow { get; set; }

        private static string GetRedirectUrl()
        {
            string AppId = "au.com.yarratrams.employeeapp";
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                return $"msauth://{AppId}/{Consts.AndroidAuthSignatureHash}";
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                AppId = "au.com.yarratrams.dingyarratrams"; // different for iOS...
                return $"msauth.{AppId}://auth";
            }
            return string.Empty;
        }

        // private constructor to ensure singleton
        private PlatformConfig()
        {
        }
    }
}
