using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Stepper.Converter
{
    public class SizePercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(double.TryParse(value.ToString(), out double size))
            {
                if (double.TryParse(parameter.ToString(), out double param))
                {
                    return size * param;
                }
                else return size;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
