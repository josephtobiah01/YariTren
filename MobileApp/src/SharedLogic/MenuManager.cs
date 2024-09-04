using Core;
using Core.Analytics;
using Core.Helpers;
using Core.Interfaces;
using Core.Models;
using Data.SharePoint.Authentication;
using Data.SharePoint.Services;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SharedLogic
{
    public class MenuManager
    {
        private readonly IClientManager _clientManager;

        public MenuManager(IClientManager clientManager) 
        {
            _clientManager = clientManager;
        }
        public async Task<ObservableCollection<Menus>> GetMenuItems(UserProfile userProfile)
        {
            var myToolsLinks = await GetMyTools(userProfile);
            return new ObservableCollection<Menus>
            {
                new Menus() {
                    Title = "News",
                    SubMenus = new ObservableCollection<Menus> {
                        new Menus() {Title = "Alerts/Notices", Url = Consts.AlertsNotices },
                        new Menus() {Title = "News Archive", Url = Consts.ArchiveNewsUrl },
                        new Menus() {Title = "Wire Archive", Url = Consts.ArchiveWireUrl }
                    }
                    },
                myToolsLinks              
            };
        }

        public async Task<Menus> GetMyTools(UserProfile userProfile)
        {

            var myToolsMenus = new Menus() { Title = "My Tools" };
            try
            {
                var client = new MyToolsClient(_clientManager);
                var myToolsLinks = await client.GetAllForCurrentUser(userProfile);

                foreach (var myToolsLink in myToolsLinks)
                {
                    if (myToolsLink.ParentMenuID == null)
                    {
                        var topMenuItem = new Menus() { Title = myToolsLink.Title, Url = myToolsLink.LinkDestination };
                        ProcessChildren(myToolsLink, myToolsLinks.ToList(), topMenuItem);
                        if (myToolsMenus.SubMenus == null)
                        {
                            myToolsMenus.SubMenus = new ObservableCollection<Menus>();
                        }
                        myToolsMenus.SubMenus.Add(topMenuItem);
                    }
                }
                return myToolsMenus;
            }
            catch (Exception ex)
            {
                var exceptionMethodParent = new StackFrame(1).GetMethod().Name;
                string message = "There was an unexpected error building the MyTools Links";
                var properties = new Dictionary<string, string> { { "ExceptionMethodParent", exceptionMethodParent }, { "Message", message } };
                //AnalyticsService.ReportException(ex, properties);
                return myToolsMenus; // return just a blank menu item for MyTools if we get here...
            }
        }

        internal static void ProcessChildren(MyToolLink Parent, List<MyToolLink> myToolsLinks, Menus ParentMenuItem)
        {
            var children = myToolsLinks.FindAll(x => x.ParentMenuID == Parent.Id).OrderBy(x => x.DisplayOrder).ToList();
            foreach (var child in children)
            {
                var menuItem = new Menus() { Title = child.Title, Url = child.LinkDestination };
                ProcessChildren(child, myToolsLinks, menuItem);
                if (ParentMenuItem.SubMenus == null)
                {
                    ParentMenuItem.SubMenus = new ObservableCollection<Menus>();
                }
                ParentMenuItem.SubMenus.Add(menuItem);
            }
        }

        public static string GetHelpUrl(ObservableCollection<Menus> menus)
        {
            var helpUrl = GetUrl(menus, "help");
            if (!string.IsNullOrEmpty(helpUrl)) return helpUrl;
            return HttpUtility.ConcatUrls(Consts.SiteUrl, Consts.HelpPageUrl);
        }

        public static string GetChangePasswordUrl(ObservableCollection<Menus> menus)
        {
            var url = GetUrl(menus, "changepassword");
            if (!string.IsNullOrEmpty(url)) return url;
            return Consts.PasswordChangeUrl;
        }


        public static string GetUrl(ObservableCollection<Menus> menus, string title)
        {
            var url = string.Empty;
            var menuItem = menus.FirstOrDefault(x => x.Title.ToLower() == title);
            if (menuItem != null) return menuItem.Url;
            foreach (var menu in menus)
            {
                if (menu.SubMenus != null)
                {
                    url = GetUrl(menu.SubMenus, title);
                    if (!string.IsNullOrEmpty(url)) return url;
                }
            }
            return string.Empty;
        }

    }

    public class MyToolsClient 
    {
        private IClientManager _clientManager;
        public string ODataQuery { get; set; }
        public string ListName { get; set; }
        public MyToolsClient(IClientManager clientManager)
        {
            ListName = "My Tools Links";
            string selectFields = "Id,Title,LinkType,LinkInheritance,LinkDestination,DisplayOrder,UserAudience,GeoCode,ParentMenuID,LocationAudience";
            ODataQuery = string.Format("?$top=100&$select={0}", selectFields);
            _clientManager = clientManager;
        }

        public async Task<IList<MyToolLink>> GetAllForCurrentUser(UserProfile userProfile)
        {
            string filterForCurrentUser = string.Format("$filter=(UserAudience eq '{0}' or UserAudience eq null) and (LocationAudience eq '{1}' or LocationAudience eq null)", userProfile.EmployeeRole, userProfile.Office);
            if (string.IsNullOrEmpty(ODataQuery))
            {
                ODataQuery = string.Format("?{0}", filterForCurrentUser);
            }
            else
            {
                ODataQuery = string.Format("{0}&{1}", ODataQuery, filterForCurrentUser);
            }
            //return GetAll();
            return await _clientManager.GetAllItems<MyToolLink>("My Tools Links", ODataQuery);
        }
    }

}
