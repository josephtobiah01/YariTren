using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Helpers
{
    public class Utility
    {
        public static string EncodeString(string s)
        {
            return System.Net.WebUtility.HtmlEncode(s);
        }

        public static string ConvertStringToTag(string s)
        {
            if (string.IsNullOrEmpty(s)) return string.Empty;
            s = s.Replace(' ', '_'); // remove spaces
            s = s.Replace("&", string.Empty); // remove ampersands
            return s;
        }
    }
}
