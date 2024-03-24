using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using MusicPlayUI.Core.Models;
using MusicPlayUI.MVVM.ViewModels;

namespace MusicPlayUI.Converters
{
    public class ToTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is NavigationModel navigationModel)
            {
                return navigationModel.ViewModel.GetType() == typeof(NowPlayingViewModel);
            }
            return value?.GetType();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
