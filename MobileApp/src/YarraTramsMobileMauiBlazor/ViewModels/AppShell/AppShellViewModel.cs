using CommunityToolkit.Mvvm.Messaging;
using Core.Interfaces;
using Core.Models;
using SharedLogic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using YarraTramsMobileMauiBlazor.Interfaces;
using YarraTramsMobileMauiBlazor.Services;
using YarraTramsMobileMauiBlazor.ViewModels.Login;
using YarraTramsMobileMauiBlazor.Views.Login;

namespace YarraTramsMobileMauiBlazor.ViewModels.AppShell
{
    public class AppShellViewModel
    {
        private string _firstName;
        private string _lastName;
        private string _versionText;
        private ObservableCollection<Menus> _menuItems;
        private INavigationService? _navigationService => ServiceLocator.Current?.GetService<INavigationService>();
        private IUserProfileService? _userProfileService => ServiceLocator.Current?.GetService<IUserProfileService>();
        private IClientManager? _clientManager => ServiceLocator.Current?.GetService<IClientManager>();
        public string FirstName
        {
            get { return _firstName; }
            set { SetPropertyValue(ref _firstName, value); }
        }
        public string LastName
        {
            get { return _lastName; }
            set { SetPropertyValue(ref _lastName, value); }
        }
        public ObservableCollection<Menus> MenuItems
        {
            get { return _menuItems; }
            set { SetPropertyValue(ref _menuItems, value); }
        }
        public string VersionText 
        {
            get { return _versionText; }
            set { SetPropertyValue(ref _versionText, value); }
        }
        public UserProfile UserProfile { get; set; }
        public Command SignOut { get; set; }
        public Command Help { get; set; }
        public Command ChangePassword { get; set; }

        public AppShellViewModel()
        {
            SignOut = new Command(SignOutCommand);
            Help = new Command(HelpCommand);
            ChangePassword = new Command(ChangePasswordCommand);
            GetFirstNameLastName();
            GetVersion();
        }

        private async void GetFirstNameLastName()
        {
            UserProfile = await _userProfileService.GetCurrentUserProfile();
            FirstName = UserProfile.FirstName;
            LastName = UserProfile.LastName;
        }

        private void GetVersion()
        {
            VersionText = $"Ding! Version {VersionTracking.CurrentVersion}";
        }

        private async Task<ObservableCollection<Menus>> GetMenuItems()
        {
            UserProfile = await _userProfileService.GetCurrentUserProfile();
            var menuManager = new MenuManager(_clientManager);
            var menuItems = await menuManager.GetMenuItems(UserProfile);
            return menuItems;

        }
        private async void SignOutCommand(object obj)
        {
            Dictionary<string, object> _param = new Dictionary<string, object>();

            _param.Add("InitialViewState", AuthenticateViewState.PinAuth);

            Shell.Current.FlyoutIsPresented = false;

            // TODO: remove all pages from stack...
            //var stack = Shell.Current.Navigation.NavigationStack.ToArray();
            //for (int i = stack.Length - 1; i > 0; i--)
            //{
            //    Shell.Current.Navigation.RemovePage(stack[i]);
            //}
            if (_navigationService != null)
            {
                await _navigationService.NavigateTo(nameof(PinPage), _param);
            }
        }

        private async void HelpCommand(object obj)
        {
            try
            {
                MenuItems = await GetMenuItems();
                string url = MenuManager.GetHelpUrl(MenuItems);
                WeakReferenceMessenger.Default.Send(url);
                Shell.Current.FlyoutIsPresented = false;
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error", $"{ex.Message}", "Ok");
            }
        }

        private async void ChangePasswordCommand(object obj)
        {
            try
            {
                MenuItems = await GetMenuItems();
                string url = MenuManager.GetChangePasswordUrl(MenuItems);
                WeakReferenceMessenger.Default.Send(url);
                Shell.Current.FlyoutIsPresented = false;
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Error", $"{ex.Message}", "Ok");
            }
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

        protected void OnPropertyChanged(string info)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
        }

    }
}
