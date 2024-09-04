using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface ICookieService
    {
        void ClearAllCookies();
        bool HasCookies();
    }
}
