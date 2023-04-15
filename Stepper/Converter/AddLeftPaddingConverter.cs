using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace Stepper.Converter
{
    internal class AddLeftPaddingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Thickness padding))
            {
                return value;
            }

            if (!double.TryParse(parameter.ToString(), out double amount))
            {
                return value;
            }

            padding.Left += amount;

            return padding;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
