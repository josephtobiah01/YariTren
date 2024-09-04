using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTramsMobileMauiBlazor.Interfaces;
using YarraTramsMobileMauiBlazor.Services;
using YarraTramsMobileMauiBlazor.ViewModels.WebView;

namespace YarraTramsMobileMauiBlazor.CustomHandlers
{
    public class SourceChangedEventArgs : EventArgs
    {
        public WebViewSource Source
        {
            get;
            private set;
        }

        public string Url
        {
            get;
            private set;
        }

        public SourceChangedEventArgs(WebViewSource source)
        {
            Source = source;
            Url = string.Empty;
        }

        public SourceChangedEventArgs(string url)
        {
            Url = url;
            Source = new UrlWebViewSource() { Url = url };
        }
    }

    public class JavaScriptActionEventArgs : EventArgs
    {
        public string Payload { get; private set; }

        public JavaScriptActionEventArgs(string payload)
        {
            Payload = payload;
        }
    }

    public interface IHybridWebView : IView
    {
        event EventHandler<SourceChangedEventArgs> SourceChanged;
        event EventHandler<JavaScriptActionEventArgs> JavaScriptAction;
        event EventHandler<EvaluateJavaScriptAsyncRequest> RequestEvaluateJavaScript;

        void Refresh();

        WebViewSource Source { get; set; }

        void Cleanup();

        void InvokeAction(string data);

    }
    public class HybridWebView : WebView, IHybridWebView
    {
        public event EventHandler<SourceChangedEventArgs> SourceChanged;
        public event EventHandler<JavaScriptActionEventArgs> JavaScriptAction;
        public event EventHandler<EvaluateJavaScriptAsyncRequest> RequestEvaluateJavaScript;

        Action<string>? action;
        Action<string>? actionNavigating;
        Action<string>? actionNavigated;

        public static readonly BindableProperty UriProperty = BindableProperty.Create(
            propertyName: "Uri",
            returnType: typeof(string),
            declaringType: typeof(HybridWebView),
            defaultValue: default(string));

        public static readonly BindableProperty URLProperty = BindableProperty.Create(
            propertyName: "URL",
            returnType: typeof(string),
            declaringType: typeof(HybridWebView),
            defaultValue: default(string),
            propertyChanged: OnURLChanged);

        public static readonly BindableProperty JavaScriptProperty = BindableProperty.Create(
            propertyName: "JavaScript",
            returnType: typeof(string),
            declaringType: typeof(HybridWebView),
            defaultValue: default(string));

        public static readonly BindableProperty RouteURLProperty = BindableProperty.Create(
            propertyName: "RouteURL",
            returnType: typeof(string),
            declaringType: typeof(HybridWebView),
            defaultValue: default(string));

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(
          propertyName: "Source",
          returnType: typeof(WebViewSource),
          declaringType: typeof(HybridWebView),
          defaultValue: new UrlWebViewSource() { Url = "about:blank" },
          propertyChanged: OnSourceChanged);

        //public WebViewSource Source
        //{
        //    get { return (WebViewSource)GetValue(SourceProperty); }
        //    set { SetValue(SourceProperty, value); }
        //}        //public WebViewSource Source
        //{
        //    get { return (WebViewSource)GetValue(SourceProperty); }
        //    set { SetValue(SourceProperty, value); }
        //}

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public string URL
        {
            get { return (string)GetValue(URLProperty); }
            set { SetValue(URLProperty, value); }
        }

        public string RouteURL
        {
            get { return (string)GetValue(RouteURLProperty); }
            set { SetValue(RouteURLProperty, value); }
        }

        public string JavaScript
        {
            get { return (string)GetValue(JavaScriptProperty); }
            set { SetValue(JavaScriptProperty, value); }
        }

        public Dictionary<string, string>? Headers { get; set; }

        public void Refresh()
        {
            if (Source == null) return;
            var s = Source;
            Source = null;
            Source = s;
        }

        private static void OnSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as HybridWebView;
            bindable.Dispatcher.Dispatch(() =>
            {
                view.SourceChanged?.Invoke(view, new SourceChangedEventArgs(newValue as WebViewSource));
            });
        }

        private static void OnURLChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var view = bindable as HybridWebView;
            bindable.Dispatcher.Dispatch(() =>
            {
                view.SourceChanged?.Invoke(view, new SourceChangedEventArgs(newValue as string));

            });
        }


        public void Cleanup()
        {
            JavaScriptAction = null;
        }

        public async void InvokeAction(string data)
        {
            WeakReferenceMessenger.Default.Send(data);
        }

        public void RegisterAction(Action<string> callback)
        {
            action = callback;
        }

        public void RegisterNavigatingAction(Action<string> callback)
        {
            actionNavigating = callback;
        }

        public void RegisterNavigatedAction(Action<string> callback)
        {
            actionNavigated = callback;
        }

     
        public void InvokeNavigatingAction(string data)
        {
            if (actionNavigating == null || data == null)
            {
                return;
            }
            actionNavigating.Invoke(data);
        }

        public void InvokeNavigatedAction(string data)
        {
            if (actionNavigated == null || data == null)
            {
                return;
            }
            actionNavigated.Invoke(data);
        }
        //public HybridWebView()
        //{
        //    this.Navigating += OnNavigating;
        //    this.Navigated += OnNavigated;
        //}

        //private void OnNavigating(object? sender, WebNavigatingEventArgs e)
        //{
        //    //InvokeNavigatingAction(e.Url);
        //    var vm = this.BindingContext as WebViewPageViewModel;
        //    if (vm != null)
        //    {
        //        vm.IsBusy = true;
        //    }

        //}

        //private void OnNavigated(object? sender, WebNavigatedEventArgs e)
        //{
        //    //InvokeNavigatedAction(e.Url);
        //    var vm = this.BindingContext as WebViewPageViewModel;
        //    if (vm != null)
        //    {
        //        vm.IsBusy = false;
        //    }
        //}
    }
}
