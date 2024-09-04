using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Core.Models
{
    public class Menus
    {
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Url { get; set; }
        public ObservableCollection<Menus> SubMenus { get; set; }
    }
}
