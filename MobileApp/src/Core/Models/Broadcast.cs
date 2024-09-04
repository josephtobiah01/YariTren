using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Models
{
    public class Broadcast : SpItem
    {
        public List<string> UserAudience { get; set; }
        public List<string> LocationAudience { get; set; }
    }

}
