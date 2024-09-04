using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface ICustomService
    {
        void CloseApplication();
        void UpdateBadgeCount();
    }
}
