using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Stepper.Converter
{
    internal class CornerRadiusConverter : IValueConverter
    {


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not CornerRadius) return new CornerRadius(0);

            CornerRadius cornerRadius = (CornerRadius)value;

            if (int.TryParse(parameter.ToString(), out int param))
            {
                if (param == 1)
                {
                    return new CornerRadius(0, cornerRadius.TopRight, 0, 0);
                }
                else if (param == 2)
                {
                    return new CornerRadius(0, 0, cornerRadius.BottomRight, 0);
                }
                else if (param == 12)
                {
                    return new CornerRadius(0, cornerRadius.TopRight, cornerRadius.BottomRight, 0);
                }
                else
                {
                    return cornerRadius;
                }
            }
            else return cornerRadius;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
