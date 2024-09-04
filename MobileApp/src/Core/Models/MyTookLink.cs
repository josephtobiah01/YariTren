using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class MyToolLink : SpItem
    {
        public string IconClass { get; set; }
        public string IconForegroundColor { get; set; }
        public string IconBackgroundColor { get; set; }
        public string LinkType { get; set; }
        public string LinkInheritance { get; set; }
        public string LinkDestination { get; set; }
        public float DisplayOrder { get; set; }
        public List<string> UserAudience { get; set; }
        public string GeoCode { get; set; }
        public float? ParentMenuID { get; set; }
        public List<string> LocationAudience { get; set; }
        public string ConfirmationText { get; set; }
    }
}
