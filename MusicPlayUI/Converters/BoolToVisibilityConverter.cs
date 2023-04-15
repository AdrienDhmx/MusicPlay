using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using Windows.UI.ViewManagement;

namespace MusicPlayUI.Converters
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public BoolToVisibilityConverter()
        {
            // set defaults
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (value is not bool && value is not int)
                return null;

            bool result;
            if (value is int @int) result = @int != 0;
            else
            {
                result = bool.Parse(value.ToString());
            }

            if (parameter is not null && int.TryParse(parameter.ToString(), out int visibility))
            {
                if(visibility < 3)
                {
                    return result ? TrueValue : (Visibility)visibility;
                }
            }
            return result ? TrueValue : FalseValue;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            if (Equals(value, TrueValue))
                return true;
            if (Equals(value, FalseValue))
                return false;
            return false;
        }
    }
}
