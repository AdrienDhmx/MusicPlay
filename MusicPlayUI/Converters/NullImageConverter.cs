using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;
using System.Windows.Media.Imaging;
using MusicFilesProcessor.Helpers;

namespace MusicPlayUI.Converters
{
    public class NullImageConverter : IValueConverter
    {
        /// <summary>
        /// The size of the cover, 0 => original, 1 => medium, 2 => Thumbnail
        /// </summary>
        public int CoverSize { get; set; } = 0;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path = value as string;
            if (path is null || !path.ValidPath())
            {
                _ = int.TryParse(parameter as string, out int defaultImage);
                return GetDefaultImage(defaultImage);
            }
            else if(CoverSize == 1)
            {
                path = ImageHelper.GetModifiedCoverPath(path, true);
            }
            else if(CoverSize == 2)
            {
                path = ImageHelper.GetModifiedCoverPath(path, false);
            }

            if (path is null || !path.ValidPath())
            {
                _ = int.TryParse(parameter as string, out int defaultImage);
                return GetDefaultImage(defaultImage);
            }
            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }

        private static string GetDefaultImage(int image)
        {
            return image switch
            {
                -1 => null,
                0 => App.defaultImage,
                1 => App.defaultArtistImage,
                _ => App.defaultImage,
            };
        }
    }
}
