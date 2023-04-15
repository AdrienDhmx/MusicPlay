using GongSolutions.Wpf.DragDrop.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MusicPlayUI.Converters
{
    public class FindScrollViewerConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null) return null;

            if(value is UIElement uIElement)
            {
                ScrollViewer scrollViewer = uIElement.GetVisualDescendent<ScrollViewer>();

                if(scrollViewer == null) return null;

                return scrollViewer;
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
