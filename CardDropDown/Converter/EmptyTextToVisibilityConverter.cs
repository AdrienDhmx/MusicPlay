using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CardDropDown.Converter
{
    public class EmptyTextToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool boolValue = value.ToString()?.Length == 0;
            if (boolValue)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
           return Binding.DoNothing;
        }
    }
}
