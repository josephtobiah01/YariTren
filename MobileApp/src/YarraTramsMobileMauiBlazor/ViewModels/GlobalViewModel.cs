using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YarraTramsMobileMauiBlazor.Interfaces;

namespace YarraTramsMobileMauiBlazor.ViewModels
{
    public class GlobalViewModel : ViewModelBase
    {
        private static GlobalViewModel _instance;
        public static GlobalViewModel Instance => _instance ??= new GlobalViewModel();
        public GlobalViewModel() : base(null)
        {
        }
    }
}
