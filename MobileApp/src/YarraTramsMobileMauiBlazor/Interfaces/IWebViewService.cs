using Core.Interfaces;


namespace YarraTramsMobileMauiBlazor.Interfaces
{
    public interface IWebViewService
    {
        Task OpenExternalAppAsync(string url);
        Task<bool> ShouldOpenInDefaultApp(string url, IConfigurationService configService);
        bool IsPayRollUrl(string url);
        bool IsVacanciesUrl(string url);
        bool IsPayRollPdf(string url);
        string CheckIsPayRollUrl(string url);
    }
}
