using Core;
using YarraTramsMobileMauiBlazor.Interfaces;
using Core.Interfaces;

namespace YarraTramsMobileMauiBlazor.Services
{
    public class WebViewService : IWebViewService
    {
        public WebViewService()
        {
            
        }

       

        public async Task OpenExternalAppAsync(string url)
        {
            Task launcherTask = Browser.OpenAsync(url, BrowserLaunchMode.External);
            await launcherTask;
            //await Launcher.OpenAsync("https://learn.microsoft.com/dotnet/maui");
        }

        public async Task<bool> ShouldOpenInDefaultApp(string url, IConfigurationService configService)
        {
            if (IsPayRollUrl(url)) return true;
            if (IsVacanciesUrl(url)) return true;

            var urlStrings = await configService.GetConfigValue("externalUrls", "");
            if (string.IsNullOrEmpty(urlStrings)) return false;
            List<string> urlStringList = urlStrings.Split(',').ToList();
            foreach (string urlString in urlStringList)
            {
                if (url.Contains(urlString, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }
        public bool IsPayRollUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            return url.Contains(Consts.PayRollBaseUrl, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsVacanciesUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;
            return url.Contains(Consts.VacanciesBaseUrl, StringComparison.OrdinalIgnoreCase);
        }

        public bool IsPayRollPdf(string url) => url.Contains("idsfiles", System.StringComparison.OrdinalIgnoreCase) && url.EndsWith("pdf", System.StringComparison.OrdinalIgnoreCase);

        public string CheckIsPayRollUrl(string url)
        {
            if (url.StartsWith("http")) return url;
            if (IsPayRollPdf(url)) return Core.Helpers.HttpUtility.ConcatUrls(Consts.PayRollBaseUrl, url);
            return url;
        }
    }
}
