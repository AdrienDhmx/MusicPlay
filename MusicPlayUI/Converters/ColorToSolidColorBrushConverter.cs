using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace MusicPlayUI.Converters
{
    public class BrushToColorConverter : IValueConverter
    {
        private readonly SolidColorBrush _defaultColor = new SolidColorBrush(Color.FromRgb(250, 250, 250));

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush)
            {
                return ((SolidColorBrush)value).Color;
            }
            else
            {
                return _defaultColor.Color;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color)
            {
                return new SolidColorBrush((Color)value);
            }
            else
            {
                return _defaultColor;
            }
        }
    }
}
