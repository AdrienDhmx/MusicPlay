using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using MusicPlay.Database.Models;

namespace MusicPlayUI.Converters
{
    public class ArtistsRelationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ObservableCollection<Artist> output = new ObservableCollection<Artist>();
            if(value is ObservableCollection<TrackArtistsRole> artists)
            {
                output = artists.Order();
                if(parameter != null && int.TryParse(parameter.ToString(), out int top) && top > 0 && output.Count > top)
                {
                    return new ObservableCollection<Artist>(output.ToList().GetRange(0, top));
                }
                return output;
            }
            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
