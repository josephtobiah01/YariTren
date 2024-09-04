using Core.Models;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IPinManager
    {
        Task SavePin(AuthenticatedUser user, SecureString pin);
        bool VerifyPin(AuthenticatedUser user, SecureString pin);
    }
}
