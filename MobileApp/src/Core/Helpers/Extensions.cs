using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Helpers
{
    public static class Extensions
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source?.IndexOf(toCheck, comp) >= 0;
        }



        /// <summary>
        /// Finds the Start of week the week, based on the supplied start day.
        /// eg:
        /// DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Monday); // would return the last Monday
        /// DateTime dt = DateTime.Now.StartOfWeek(DayOfWeek.Sunday); // would return the last Sunday
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <param name="startOfWeek">The start of week.</param>
        /// <returns></returns>
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }
    }
}
