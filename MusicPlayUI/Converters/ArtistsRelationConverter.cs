using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using MusicPlayModels.MusicModels;

namespace MusicPlayUI.Converters
{
    public class ArtistsRelationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is List<ArtistDataRelation> artists)
            {
                Debug.Print(artists.Count.ToString());

                if(parameter != null && int.TryParse(parameter.ToString(), out int top))
                {
                    if (top > 0)
                    {
                        artists = artists.Exclude(false, false, false, true, false).Order();
                        if(artists.Count > top)
                            return artists.GetRange(0, top);
                        else return artists;
                    }
                    else if (top == 0)
                        return artists.Order(false);
                }
                return artists.Order();
            }
            return new List<ArtistDataRelation>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
