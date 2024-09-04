using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YarraTramsMobileMauiBlazor.Interfaces
{
    public interface IUtilitiesService
    {
        void TrackUserAction(string data);
        void TrackCustomUserAction(string area, string message);
    }
}
