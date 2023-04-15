using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ColorCanvas.Converter
{
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is null) return new SolidColorBrush();

            Color? color = (Color)value;
            if(color is not null && color.HasValue)
            {
                return new SolidColorBrush(color.Value);
            }
            else
            {
                return new SolidColorBrush();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return Color.FromRgb(0, 0, 0); ;

            SolidColorBrush? solidColorBrush = (SolidColorBrush?)value;
            if(solidColorBrush is not null)
            {
                return solidColorBrush.Color;
            }
            else
            {
                return Color.FromRgb(0, 0, 0);
            }
        }
    }
}
