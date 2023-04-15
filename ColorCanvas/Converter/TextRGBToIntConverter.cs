using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ColorCanvas.Converter
{
    internal class TextRGBToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || string.IsNullOrWhiteSpace(value.ToString())) return 0;

            if(int.TryParse(value.ToString(), out int rgbValue))
            {
                if(rgbValue < 0 ) return 0;
                if(rgbValue > 255 ) return 255;
                return rgbValue;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || string.IsNullOrWhiteSpace(value.ToString())) return 0;

            if (int.TryParse(value.ToString(), out int rgbValue))
            {
                if (rgbValue < 0) return 0;
                if (rgbValue > 255) return 255;
                return rgbValue;
            }
            else
            {
                return 0;
            }
        }
    }
}
