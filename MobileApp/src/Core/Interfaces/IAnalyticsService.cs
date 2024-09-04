using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IAnalyticsService
    {
        void ReportEvent(string message, Dictionary<string, string> properties);
        void ReportEvent(string message, string customMessage);
        Task ReportException(Exception ex, string customMessage = "");
        Task ReportException(Exception ex, Dictionary<string, string> properties);
    }
}
