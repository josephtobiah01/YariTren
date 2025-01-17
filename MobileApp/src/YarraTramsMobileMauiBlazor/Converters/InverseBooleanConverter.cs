﻿using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace YarraTramsMobileMauiBlazor.Converters
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool val)
                return !val;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
