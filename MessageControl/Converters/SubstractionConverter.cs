using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MessageControl.Converters
{
    public class SubstractionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (int.TryParse(value.ToString(), out int originalSize) && int.TryParse(parameter.ToString(), out int substract) && originalSize > substract)
            {
                return originalSize - substract;
            }
            else
            {
                return 1;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
