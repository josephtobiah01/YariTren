using Core.Helpers;
using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace YarraTramsMobileMauiBlazor.Converters
{
    public class RosterTwoValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (string)value;
            var para = bool.Parse(parameter.ToString());

            if (!para && !string.IsNullOrEmpty(val) && !val.Contains("CDO", StringComparison.OrdinalIgnoreCase))
                return true;
            else
                return false;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

    }
}