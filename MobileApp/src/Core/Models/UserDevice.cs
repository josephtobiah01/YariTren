using System;
using System.Collections.Generic;
using System.Text;
using Core.Helpers;

namespace Core.Models
{
    public class UserDevice : SpItem
    {
        private string _devicePlatform;

        public string AppVersion { get; set; }
        public string DeviceInstallationId { get; set; }
        public string DevicePlatform {
            get
            {
                if (string.IsNullOrEmpty(_devicePlatform))
                {
                    if (DeviceOS.Contains("android", StringComparison.OrdinalIgnoreCase)) return "Google";
                    if (DeviceOS.Contains("ios", StringComparison.OrdinalIgnoreCase)) return "Apple";
                }
                return _devicePlatform;
            }
            set
            {
                if (value.Contains("Android", StringComparison.OrdinalIgnoreCase))
                {
                    _devicePlatform = "Google";
                    return;
                }
                if (value.Contains("ios", StringComparison.OrdinalIgnoreCase) || value.Contains("apple"))
                {
                    _devicePlatform = "Apple";
                    return;
                }
                _devicePlatform = value;
            }
        } // Apple/Google
        public string DeviceOS { get; set; }
        public string DeviceOSVersion { get; set; }
        public string DevicePushToken { get; set; }
        public string DeviceType { get; set; }
        public string EmployeeID { get; set; }
        public string EmployeeName { get; set; }
        public List<string> LocationAudience { get; set; }
        public string Tags { get; set; }
        public List<string> UserAudience { get; set; }
        public string Office { get; set; }
        public List<string> ManagementLevel { get; set; }
        public List<string> Function { get; set; }

    }
}
