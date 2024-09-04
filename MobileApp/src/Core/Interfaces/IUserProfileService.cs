using Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IUserProfileService
    {
        Task<UserProfile> GetCurrentUserProfile();
        Task<UserProfile> GetUserProfile(string loginName);
        bool HasUserProfile();
        void AddUserProfileToCache(UserProfile userProfile, int daysToCache = 1);
        void ResetUserDetails();
    }
}
