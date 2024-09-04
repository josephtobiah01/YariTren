using Android.Webkit;
using Android.OS;
using Android.App;
using Microsoft.Maui.ApplicationModel;
using Environment = Android.OS.Environment;
using Application = Android.App.Application;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;
using WebView = Android.Webkit.WebView;
using HttpUtility = Core.Helpers.HttpUtility;
using YarraTramsMobileMauiBlazor.CustomHandlers;
using Core.Helpers;
using Android.Graphics;
using Core.Interfaces;
using Core;
using YarraTramsMobileMauiBlazor.PageHandlers;
using YarraTramsMobileMauiBlazor.Interfaces;
using YarraTramsMobileMauiBlazor.Services;
using Android.Content;
using Android.Widget;
using CommunityToolkit.Mvvm.Messaging;
using YarraTramsMobileMauiBlazor.ViewModels.WebView;
using YarraTramsMobileMauiBlazor.Views.WebView;
using System.Net;
using Microsoft.Maui.Platform;
using YarraTramsMobileMauiBlazor.Messages;

namespace YarraTramsMobileMauiBlazor.Platforms.Android
{
    public class JavascriptWebViewClient : WebViewClient
    {
        private readonly string _javascript;
        private readonly HybridWebView? _hybridWebView;
        private AdfsLoginHandler _loginHandler = new AdfsLoginHandler(Consts.TenancyId, Consts.CustomSignInPage);
        private readonly IAnalyticsService? _analyticsService;
        private INavigationService? _navigationService => ServiceLocator.Current?.GetService<INavigationService>();
        private IWebViewService? _webViewService => ServiceLocator.Current?.GetService<IWebViewService>();
        private IClientManager? _clientManager => ServiceLocator.Current?.GetService<IClientManager>();
        //private readonly Context _context;


        public JavascriptWebViewClient(
            string javascript,
            HybridWebView hybridWebView,
            IAnalyticsService analyticsService)
        {
            _javascript = javascript;
            _hybridWebView = hybridWebView;
            _analyticsService = analyticsService;
            
        }

        public override async void OnLoadResource(WebView? view, string? url)
        {
            base.OnLoadResource(view, url);
            if (HttpUtility.IsPDFUrl(url) || IsPayRollPdf(url))
            {
                await LoadPDF(view, url);
            }
            else
            {
                view.EvaluateJavascript(_javascript, null);
            }
            //if (view != null)
            //    view.EvaluateJavascript(_javascript, null);
        }

        public override async void OnPageFinished(WebView? view, string? url)
        {
            base.OnPageFinished(view, url);
            _hybridWebView?.InvokeNavigatedAction(url);
            _hybridWebView.RouteURL = url;
        }

        public override async void OnPageStarted(WebView view, string url, Bitmap? favicon)
        {
            if (_loginHandler.CanHandle(url) && _loginHandler.Redirect(url, out string redirectUrl))
            {
                view.StopLoading();
                view.LoadUrl(redirectUrl);
                // Log analytics message:
                string message = "Redirected to a different URL";
                var properties = new Dictionary<string, string> { { "RedirectURL", redirectUrl }, { "LastPartOfUrl", redirectUrl.Substring(redirectUrl.Length - 64) }, { "Message", message } };
                _analyticsService.ReportEvent(message, properties);
            }
            else
            {
                _hybridWebView?.InvokeNavigatingAction(url);
            }

            base.OnPageStarted(view, url, favicon);
        }

        internal static bool IsPayRollPdf(string url) => url.Contains("idsfiles", System.StringComparison.OrdinalIgnoreCase) && url.EndsWith("pdf", System.StringComparison.OrdinalIgnoreCase);

        private static string CheckIsPayRollUrl(string url)
        {
            if (url.StartsWith("http")) return url;
            if (IsPayRollPdf(url)) return HttpUtility.ConcatUrls(Consts.PayRollBaseUrl, url);
            return url;
        }

        public async Task LoadPDF(WebView view, string url)
        {
            if (Environment.MediaMounted.Equals(Environment.ExternalStorageState))
            {
                string fileName = HttpUtility.GetFileNameFromUrl(url);
                var folder = Application.Context.GetExternalFilesDir(Environment.DirectoryDocuments) ?? Environment.ExternalStorageDirectory;
                if (!folder.Exists()) folder.Mkdir();
                //string filePath = $"{folder}/{fileName}";
                string filePath = System.IO.Path.Combine(folder.AbsolutePath, fileName);
                bool fileDownloaded = false;

                // Handle if it's a Payroll pdf...
                url = CheckIsPayRollUrl(url);

                //UserDialogs.Instance.ShowLoading("Loading...", null);
                await Task.Delay(100);

                try
                {
                    fileDownloaded = await _clientManager.DownloadFile(url, filePath);
                }
                catch (System.Exception ex)
                {

                    ShowAlert("Error", $"Error downloading/saving pdf file to '{filePath}' from URL: {url}");
                    Console.WriteLine(ex.Message );
                    Toast.MakeText(Application.Context, "Unable to download pdf file.", ToastLength.Long).Show();
                }
                //finally
                //{
                //    UserDialogs.Instance.HideLoading();
                //}

                if (fileDownloaded && File.Exists(filePath))
                {
                    try
                    {
                        view.Settings.AllowUniversalAccessFromFileURLs = true;
                        view.Settings.AllowFileAccess = true;
                        view.Settings.JavaScriptEnabled = true;
                        view.Settings.AllowFileAccessFromFileURLs = true;
                        view.Settings.BuiltInZoomControls = true;

                        //string androidFilePath = $"file://{filePath}";
                        
                        //string googleDocsUrl = $"https://docs.google.com/gview?embedded=true&url={androidFilePath}";
                        string androidFilePath = $"file://{WebUtility.UrlEncode(filePath)}";
                        var fileExists = File.Exists(androidFilePath);
                        var pdfjsExists = File.Exists("android_asset/pdfjs/web/viewer.html");
                        string pdfUrl = $"file:///android_asset/pdfjs/web/viewer.html?file={androidFilePath}";

                        
                        //view.LoadUrl(pdfUrl);

                        //Dictionary<string, object> param = new Dictionary<string, object>();
                        //param.Add("pdfURL", pdfUrl);

                        await _navigationService.NavigateTo(nameof(SimpleWebViewPage));
                        WeakReferenceMessenger.Default.Send<SimpleWebViewMessage>(new SimpleWebViewMessage(pdfUrl));

                        //WeakReferenceMessenger.Default.Send(new PdfFileMessage(fileName));

                    }
                    catch(Exception ex)
                    {
                        Console.Write(ex.Message );
                    }
                    
                    
                    //await _navigationService.NavigateTo(nameof(SimpleWebViewPage));
                    //WeakReferenceMessenger.Default.Send<SimpleWebViewMessage>(new SimpleWebViewMessage(filePath));
                }
                else
                {
                    Toast.MakeText(Application.Context, "Unable to download pdf file.", ToastLength.Long).Show();
                    await _webViewService.OpenExternalAppAsync(url);
                }
            }
            else
            {
                Toast.MakeText(Application.Context, "Unable to download pdf file.", ToastLength.Long).Show();
                await _webViewService.OpenExternalAppAsync(url);
            }
        }
        private void ShowAlert(string title, string message, string ok = "OK")
        {
            var context = Application.Context;
            AlertDialog.Builder alert = new AlertDialog.Builder(context);
            alert.SetTitle(title);
            alert.SetMessage(message);
            alert.SetPositiveButton("OK", (senderAlert, args) => { });

            Dialog dialog = alert.Create();
            dialog.Show();
        }

    }
}
