using CommunityToolkit.Mvvm.Messaging;
using Core;
using Core.Analytics;
using Core.Interfaces;
using Core.Models;
using Data.SharePoint.Clients;
using SharedLogic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using YarraTramsMobileMauiBlazor.Interfaces;
using YarraTramsMobileMauiBlazor.Services;

namespace YarraTramsMobileMauiBlazor.Views.Flyout;

public partial class FlyoutContentPage : ContentView, INotifyPropertyChanged
{
    private ObservableCollection<Menus> _menuItems;
    private ObservableCollection<Menus> _subMenuItems;
    private ObservableCollection<Menus> _subSubMenuItems;
    private bool _isSubMenu;
    private string _parentMenu;
    private IUserProfileService? _userProfileService => ServiceLocator.GetService<IUserProfileService>();
    private IAnalyticsService _analyticsService => ServiceLocator.GetService<IAnalyticsService>();
    private IConfigurationService _configurationService => ServiceLocator.GetService<IConfigurationService>();
    private IWebViewService _webViewService => ServiceLocator.GetService<IWebViewService>();
    private IClientManager? _clientManager => ServiceLocator.Current?.GetService<IClientManager>();
    private IUtilitiesService? _utilitiesService => ServiceLocator.Current?.GetService<IUtilitiesService>();

    public bool IsSubmenu 
    { 
        get {  return _isSubMenu; }
        set { SetPropertyValue(ref _isSubMenu, value); }
    }
    private int Level { get; set; }
    public string VersionText { get; set; }
    public string ParentMenuHeight { get; set; } = "0";
    public ObservableCollection<Menus> MenuItems 
    {
        get { return _menuItems; }
        set { SetPropertyValue(ref _menuItems, value); }
    }
    public ObservableCollection<Menus> SubMenuItems 
    { 
        get { return _subMenuItems; }
        set { SetPropertyValue(ref _subMenuItems, value); } 
    }
    public ObservableCollection<Menus> SubSubMenuItems 
    {
        get { return _subSubMenuItems; }
        set { SetPropertyValue(ref _subSubMenuItems, value); }
    }
    public string ParentMenu 
    {
        get { return _parentMenu; }
        set { SetPropertyValue(ref _parentMenu, value); }
    }
    private string OldParentMenu { get; set; }
    private string OldOldParentMenu { get; set; }
    protected UserProfile UserProfile { get; set; }
    public FlyoutContentPage()
	{
		InitializeComponent();
        BindingContext = this;
        MenuItems = new ObservableCollection<Menus>();
        SubMenuItems = new ObservableCollection<Menus>();
        SubSubMenuItems = new ObservableCollection<Menus>();
        SetControls();
	}

    private async void SetControls()
    {
        var menuManager = new MenuManager(_clientManager);
        //var configManager = new ConfigManager();
        UserProfile = await _userProfileService.GetCurrentUserProfile();
        var menuItems = await menuManager.GetMenuItems(UserProfile);
        foreach (var menu in menuItems)
        {
            MenuItems.Add(menu);
        }
        //listMenu.ItemsSource = MenuItems;
        VersionText = $"Ding! Version {VersionTracking.CurrentVersion}";
        ParentMenuHeight = "0";
        
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        try
        {
            Level--;
            switch (Level)
            {
                case 0:
                    IsSubmenu = false;
                    ParentMenuHeight = "0";
                    this.listMenu.ItemsSource = MenuItems;
                    break;
                case 1:
                    ParentMenu = OldParentMenu;
                    listMenu.ItemsSource = SubMenuItems;
                    break;
                case 2:
                    ParentMenu = OldOldParentMenu;
                    listMenu.ItemsSource = SubSubMenuItems;
                    break;
            }
            //UpdateMenuBasedOnLevel();
        }
        catch (Exception ex)
        {
            Application.Current.MainPage.DisplayAlert("Error", $"{ex.Message}", "Ok");
        }
    }

    //void Handle_Left_Arrow_Tapped(object sender, System.EventArgs e)
    //{
        
    //}

    private async void listMenu_ItemTapped(object sender, ItemTappedEventArgs e)
    {
        try
        {
            var listView = (ListView)sender;
            var sumMenus = (listView.SelectedItem as Menus).SubMenus;
            if (sumMenus != null && sumMenus.Count > 0)
            {
                IsSubmenu = true;
                ParentMenuHeight = "Auto";
                Level++;
                if (Level <= 2)
                {
                    OldParentMenu = ParentMenu;
                    ParentMenu = (listView.SelectedItem as Menus).Title;
                    SubMenuItems = (ObservableCollection<Menus>)listMenu.ItemsSource;
                    listMenu.ItemsSource = (listView.SelectedItem as Menus).SubMenus;
                }
                else if (Level > 2)
                {
                    OldOldParentMenu = ParentMenu;
                    ParentMenu = (listView.SelectedItem as Menus).Title;
                    SubSubMenuItems = (ObservableCollection<Menus>)listMenu.ItemsSource;
                    listMenu.ItemsSource = (listView.SelectedItem as Menus).SubMenus;
                }
            }
            else
            {
                var url = (listView.SelectedItem as Menus).Url;
                if (!string.IsNullOrEmpty(url))
                {
                    if (url.Contains("staff-benefits"))
                    {
                        url = Consts.StaffBenefitsUrl;
                    }
                    else if (await _webViewService.ShouldOpenInDefaultApp(url, _configurationService))
                    {
                        await _webViewService.OpenExternalAppAsync(url);
                        return;
                    }
                    if (url.Contains("roster", StringComparison.OrdinalIgnoreCase))
                    {
                        _utilitiesService.TrackUserAction("Roster");
                    }
                    WeakReferenceMessenger.Default.Send(url);
                    Shell.Current.FlyoutIsPresented = false;
                }
            }
            listView.SelectedItem = null;
        }
        catch (Exception ex)
        {
            Application.Current.MainPage.DisplayAlert("Error", $"{ex.Message}", "Ok");
        }
    }

    //async void Handle_ItemTapped(object sender, System.EventArgs e)
    //{
        
    //}

    public static bool IsPayRollUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return false;
        return url.Contains(Consts.PayRollBaseUrl, StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsVacanciesUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return false;
        return url.Contains(Consts.VacanciesBaseUrl, StringComparison.OrdinalIgnoreCase);
    }


    //public async void TrackUserAction(string data)
    //{

    //    string message = string.Format("{0} - opened", data);
    //    string role = "Not set";
    //    string location = "Not set";
    //    var user = await _userProfileService.GetCurrentUserProfile();
    //    if (user != null)
    //    {
    //        if (!string.IsNullOrEmpty(user.EmployeeRole)) role = user.EmployeeRole;
    //        if (!string.IsNullOrEmpty(user.Office)) location = user.Office;
    //    }
    //    var properties = new Dictionary<string, string> { { "Area", data }, { "Message", message }, { "Role", role }, { "Location", location } };
    //    _analyticsService.ReportEvent(message, properties);
    //}

    public void ResetState()
    {
        // Clear menu items
        MenuItems.Clear();
        SubMenuItems.Clear();
        SubSubMenuItems.Clear();

        // Reset other properties to their default values
        IsSubmenu = false;
        Level = 0;
        ParentMenuHeight = "0";
        ParentMenu = string.Empty;
        OldParentMenu = string.Empty;
        OldOldParentMenu = string.Empty;
        VersionText = $"Ding! Version {VersionTracking.CurrentVersion}";

        // Clear ListView selection
        listMenu.SelectedItem = null;

        // Reinitialize controls or data if needed
        SetControls();
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected bool SetPropertyValue<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
    {
        if (Equals(field, value))
        {
            return false;
        }
        else
        {
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    private void UpdateMenuBasedOnLevel()
    {
        switch (Level)
        {
            case 0:
                IsSubmenu = false;
                ParentMenuHeight = "0";
                this.listMenu.ItemsSource = MenuItems;
                break;
            case 1:
                ParentMenu = OldParentMenu;
                listMenu.ItemsSource = SubMenuItems;
                break;
            case 2:
                ParentMenu = OldOldParentMenu;
                listMenu.ItemsSource = SubSubMenuItems;
                break;
        }
    }

    protected void OnPropertyChanged(string info)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
    }

    private void listMenu_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem == null)
            return;

        // Deselect the item
        ((ListView)sender).SelectedItem = null;
    }
}