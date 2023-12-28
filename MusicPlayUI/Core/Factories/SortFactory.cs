using MusicPlay.Language;
using MusicPlayModels;
using MusicPlayModels.Enums;
using MusicPlayModels.MusicModels;
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
            if(typeof(T) == typeof(AlbumModel))
            {
                return GetAlbumSorting();
            }
            else if (typeof(T) == typeof(ArtistModel))
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
                new(SortEnum.AZ, Resources.Title, false, false),
                new(SortEnum.Year, Resources.Year, false, false),
                new(SortEnum.MostPlayed, Resources.Most_Played, false, false),
                new(SortEnum.LastPlayed, Resources.Last_Played, false, false),
                new(SortEnum.AddedDate, "Added Date", false, false),
                new(SortEnum.UpdatedDate, "Last Update Date", false, false),

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
                new(SortEnum.AZ, Resources.Title, false, false),
                new(SortEnum.MostPlayed, Resources.Most_Played, false, false),
                new(SortEnum.LastPlayed, Resources.Last_Played, false, false),
                new(SortEnum.AddedDate, "Added Date", false, false),
                new(SortEnum.UpdatedDate, "Last Update Date", false, false),

            };

            if (values.Length > 1 && int.TryParse(values[0], out int enumVAlue))
            {
                SortEnum sortEnum = (SortEnum)enumVAlue;
                isAscending = int.Parse(values[1]) == 1;

                foreach (SortModel sort in output)
                {
                    if(sort.SortType == sortEnum)
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
