using Microsoft.AppCenter.Crashes;
using AppCentre = Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Core.Interfaces;
using System.Threading.Tasks;

namespace Core.Analytics
{
    public class AnalyticsService : IAnalyticsService
    {
        public void ReportEvent(string message, Dictionary<string, string> properties)
        {
            AppCentre.Analytics.TrackEvent(message, properties);
            
        }

        public void ReportEvent(string message, string customMessage)
        {
            var properties = new Dictionary<string, string> { { "Message", customMessage } };
            ReportEvent(message, properties);
        }

        public async Task ReportException(Exception ex, string customMessage = "")
        {
            var properties = new Dictionary<string, string> { { "Message", customMessage } };
            await ReportException(ex, properties);
        }

        public async Task ReportException(Exception ex, Dictionary<string, string> properties)
        {
            var message = string.Format("ERROR - from: '{0}', message: {1}", ex.TargetSite, ex.Message);
            await Task.Run(() => Crashes.TrackError(ex, properties)); 
        }
    }
}
