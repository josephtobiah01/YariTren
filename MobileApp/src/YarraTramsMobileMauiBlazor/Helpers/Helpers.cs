using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace YarraTramsMobileMauiBlazor.Helpers
{
    public static class Helpers
    {
        public static bool IsAlphanumeric(this string TextContent)
        {
            String userIdPattern = @"^[a-zA-Z0-9_\-]*$";

            //Regex regex = new Regex(@"^([\w\.\-]+)@([\w\.\-]+)((\.(\w){2,3})+)$");
            Regex regex = new Regex(userIdPattern);

            Match match = regex.Match(TextContent);

            return match.Success;
        }

        public static bool IsValidEmail(this string TextContent)
        {
            String theEmailPattern = @"^[\w!#$%&'*+\-/=?\^_`{|}~]+(\.[\w!#$%&'*+\-/=?\^_`{|}~]+)*"
                                   + "@"
                                   + @"((([\-\w]+\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\.){3}[0-9]{1,3}))$";

            //Regex regex = new Regex(@"^([\w\.\-]+)@([\w\.\-]+)((\.(\w){2,3})+)$");
            Regex regex = new Regex(theEmailPattern);

            Match match = regex.Match(TextContent);

            return match.Success;

        }

        public static bool IsValidName(this string TextContent)
        {
            String namePattern = @"^[\p{L}\p{M}' \.\-]+$";

            //Regex regex = new Regex(@"^([\w\.\-]+)@([\w\.\-]+)((\.(\w){2,3})+)$");
            Regex regex = new Regex(namePattern);

            Match match = regex.Match(TextContent);

            return match.Success;

        }


        public static bool IsValidPhoneNumber(this string TextContent)
        {
            Regex digitsOnly = new Regex(@"[^\d]");
            var _cleanContent = digitsOnly.Replace(TextContent, "");

            return _cleanContent.Length >= 11 && _cleanContent.Length <= 13;
        }
    }
}
