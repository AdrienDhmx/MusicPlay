using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MusicPlayUI.Converters
{
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return bool.TryParse(value.ToString(), out bool Value) ? !Value : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return bool.TryParse(value.ToString(), out bool Value) ? !Value : value;
        }
    }
}
