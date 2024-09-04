using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IMyMenuToolsService
    {
        Task<List<Menus>> GetMenuItems(UserProfile userProfile);
        Task<string> GetChangePasswordUrl(List<Menus> menus);
        Task<string> GetHelpUrl(List<Menus> menus);
    }
}
