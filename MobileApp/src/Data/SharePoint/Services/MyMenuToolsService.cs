using Core;
using Core.Helpers;
using Core.Interfaces;
using Core.Models;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.SharePoint.Clients
{
    public class MyMenuToolsService : IMyMenuToolsService
    {
        private readonly IClientManager _clientManager;
        private readonly string _siteUrl;

        public MyMenuToolsService(
            IClientManager clientManager,
            string s)
        {
            _clientManager = clientManager;

        }

        private List<Menus> GetChildMenus(MyToolLink parent, List<MyToolLink> links)
        {
            return links.Where(link => link.ParentMenuID == parent.Id)
                        .OrderBy(link => link.DisplayOrder)
                        .Select(link => new Menus
                        {
                            Title = link.Title,
                            Url = link.LinkDestination,
                            SubMenus = GetChildMenus(link, links)
                        })
                        .ToList();
        }
        public async Task<Menus> GetMyTools(UserProfile userProfile)
        {
            string filterForCurrentUser =
                $"$filter=(UserAudience eq '{userProfile.EmployeeRole}' or UserAudience eq null) and " +
                $"(LocationAudience eq '{userProfile.Office}' or LocationAudience eq null)";

            string query = $"?$top=100&$select=Id,Title,LinkType,LinkInheritance,LinkDestination,DisplayOrder,UserAudience,GeoCode,ParentMenuID,LocationAudience&{filterForCurrentUser}";

            var myToolsLinks = await _clientManager.GetAllItems<MyToolLink>(ListName, query);

            var myToolsMenus = new Menus { Title = "My Tools" };
            myToolsMenus.SubMenus = myToolsLinks
                .Where(link => link.ParentMenuID == null)
                .Select(link => new Menus
                {
                    Title = link.Title,
                    Url = link.LinkDestination,
                    SubMenus = GetChildMenus(link, myToolsLinks)
                })
                .ToList();

            return myToolsMenus;
        }

        public async Task<List<Menus>> GetMenuItems(UserProfile userProfile)
        {
            var myToolsLinks = await GetMyTools(userProfile);
            return new List<Menus>
            {
                new Menus
                {
                    Title = "News",
                    SubMenus = new List<Menus>
                    {
                        new Menus { Title = "Alerts/Notices", Url = Consts.AlertsNotices },
                        new Menus { Title = "News Archive", Url = Consts.ArchiveNewsUrl },
                        new Menus { Title = "Wire Archive", Url = Consts.ArchiveWireUrl }
                    }
                },
                myToolsLinks
            };

            string query = $"?_api/web/lists/getbytitle('Menus')/items";
            var menuItems = await _clientManager.GetAllItems<Menus>(query);
            return menuItems;
        }

        public static string GetHelpUrl(List<Menus> menus)
        {
            var helpUrl = GetUrl(menus, "help");
            if (!string.IsNullOrEmpty(helpUrl)) return helpUrl;
            return HttpUtility.ConcatUrls(Consts.SiteUrl, Consts.HelpPageUrl);
        }

        public static string GetChangePasswordUrl(List<Menus> menus)
        {
            var url = GetUrl(menus, "changepassword");
            if (!string.IsNullOrEmpty(url)) return url;
            return Consts.PasswordChangeUrl;
        }


        public static string GetUrl(List<Menus> menus, string title)
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
}
