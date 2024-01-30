using MusicPlay.Database.Enums;
using MusicPlay.Database.Models;
using MusicPlay.Language;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services;
using MusicPlayUI.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayUI.Core.Factories
{
    public static class SortFactory
    {
        public static ObservableCollection<SortModel> GetSortMenu<T>() where T : PlayableModel
        {
            if(typeof(T) == typeof(Album))
            {
                return GetAlbumSorting();
            }
            else if (typeof(T) == typeof(Artist))
            {
                return GetArtistsSorting();
            }

            return null;
        }

        private static ObservableCollection<SortModel> GetAlbumSorting()
        {
            // typeId/IsAscending => int/int
            string Selectedsorting = ConfigurationService.GetStringPreference(SettingsEnum.AlbumOrder);
            string[] values = Selectedsorting.Split('/');
            bool isAscending;

            ObservableCollection<SortModel> output = new()
            {
                new SortModel(1, SortEnum.AZ, Resources.Title, false, false),
                new(2, SortEnum.Year, Resources.Year, false, false),
                new(3, SortEnum.MostPlayed, Resources.Most_Played, false, false),
                new(4, SortEnum.LastPlayed, Resources.Last_Played, false, false),
                new(7, SortEnum.AddedDate, "Added Date", false, false),
                new(6, SortEnum.UpdatedDate, "Last Update Date", false, false),

            };

            if (values.Length > 1 && int.TryParse(values[0], out int enumVAlue))
            {
                SortEnum sortEnum = (SortEnum)enumVAlue;
                isAscending = int.Parse(values[1]) == 1;

                output[(int)sortEnum].IsSelected = true;
                output[(int)sortEnum].IsAscending = isAscending;
                return output;
            }

            output[0].IsAscending = true;
            output[0].IsSelected = true;

            return output;
        }

        private static ObservableCollection<SortModel> GetArtistsSorting()
        {
            string Selectedsorting = ConfigurationService.GetStringPreference(SettingsEnum.ArtistOrder);
            string[] values = Selectedsorting.Split('/');
            bool isAscending;

            ObservableCollection<SortModel> output = new()
            {
                new(1, SortEnum.AZ, Resources.Title, false, false),
                new(2, SortEnum.MostPlayed, Resources.Most_Played, false, false),
                new(3, SortEnum.LastPlayed, Resources.Last_Played, false, false),
                new(4, SortEnum.AddedDate, "Added Date", false, false),
                new(5, SortEnum.UpdatedDate, "Last Update Date", false, false),

            };

            if (values.Length > 1 && int.TryParse(values[0], out int enumVAlue))
            {
                SortEnum sortEnum = (SortEnum)enumVAlue;
                isAscending = int.Parse(values[1]) == 1;

                foreach (SortModel sort in output)
                {
                    if(sort.Type == sortEnum)
                    {
                        sort.IsSelected = true;
                        sort.IsAscending = isAscending;
                        return output;
                    }
                }
                
            }

            output[0].IsAscending = true;
            output[0].IsSelected = true;

            return output;
        }
    }
}
