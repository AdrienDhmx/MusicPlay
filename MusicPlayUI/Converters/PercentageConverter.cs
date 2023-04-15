using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace MusicPlayUI.Converters
{
    public class PercentageConverter : IValueConverter
    {
        public bool Invert { get; set; } = false;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(double.TryParse(value.ToString(), out var percentage))
            {
                if(Invert)
                    return (100d - percentage) / 100d;
                return percentage / 100d;
            }
            else
            {
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
