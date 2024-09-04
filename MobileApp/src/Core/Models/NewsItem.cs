using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class NewsItem : SpItem
    {
        public List<string> UserAudience { get; set; } = new List<string>();
        public List<string> LocationAudience { get; set; } = new List<string>();
        public List<string> Function { get; set; } = new List<string>();
        public List<string> ManagementLevel { get; set; } = new List<string>();
    }
}
