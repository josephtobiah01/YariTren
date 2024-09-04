using System;
using System.Collections.Generic;
using System.Text;

namespace Data.SharePoint.Clients
{
    interface IClient<T>
    {
        IList<T> GetAll();
        int GetCount();
    }
}
