using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MusicPlayUI.Converters
{
    public class SubstractionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double size = (double.TryParse(value.ToString(), out double ValueInt) && int.TryParse(parameter.ToString(), out int substractor)) ? ValueInt - substractor : ValueInt;
            return size >= 0 ? size : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
