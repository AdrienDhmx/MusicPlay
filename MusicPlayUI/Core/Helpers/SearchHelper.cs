using DataBaseConnection.DataAccess;
using MusicPlayModels.Enums;
using MusicPlayModels.MusicModels;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services;
using MusicPlayUI.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayUI.Core.Helpers
{
    public static class SearchHelper
    {
        public static ObservableCollection<FilterModel> GetSelectedFilters(bool album = true)
        {
            SettingsEnum settingsEnum = album ? SettingsEnum.AlbumFilter : SettingsEnum.ArtistFilter;

            ObservableCollection<FilterModel> output = new();

            // string look like this:
            // FilterTypeInt:id.id.id/FilterTypeInt:id.id.id/
            // 3:2.4/1:42
            string stringFilter = ConfigurationService.GetStringPreference(settingsEnum);
            if (!string.IsNullOrWhiteSpace(stringFilter) && stringFilter != "NONE")
            {
                string[] FilterTypes = stringFilter.Split('/');

                if (FilterTypes is not null && FilterTypes.Length > 0)
                {
                    foreach (string filterType in FilterTypes)  
                    {
                        if (string.IsNullOrWhiteSpace(filterType)) continue;

                        string[] values = filterType.Split(':');
                        if(values.Length <= 1) continue;

                        if (!int.TryParse(values[0], out int enumValue)) continue;
                        FilterEnum filterEnum = (FilterEnum)enumValue;

                        if(string.IsNullOrWhiteSpace(values[1])) continue;

                        string[] valuesId = values[1].Split('.');
                        foreach (string filterValue in valuesId)
                        {
                            if (string.IsNullOrWhiteSpace(filterValue)) continue;

                            output.Add(new(int.Parse(filterValue), filterEnum));
                        }
                    }
                }
            }

            return output;
        }

        public static async Task<List<ArtistModel>> FilterArtists(List<FilterModel> filters, string searchString, SortEnum sortEnum, bool ascending = false)
        {
            SaveFilter(filters, SettingsEnum.ArtistFilter);
            SaveOrder(SettingsEnum.ArtistOrder, sortEnum, ascending);

            List<int> artistTypes = new();
            List<int> tagTypes = new();


            foreach (FilterModel filter in filters)
            {
                switch (filter.FilterType)
                {
                    case FilterEnum.ArtistType:
                        artistTypes.Add(filter.ValueId);
                        break;
                    case FilterEnum.Genre:
                        tagTypes.Add(filter.ValueId);
                        break;
                    default:
                        break;
                }
            }

            return await DataAccess.Connection.SearchArtists(tagTypes, artistTypes, searchString, sortEnum, ascending);
        }

        public static async Task<List<AlbumModel>> FilterAlbum(List<FilterModel> filters, string searchString, SortEnum sortEnum, bool ascending = false)
        {
            SaveFilter(filters, SettingsEnum.AlbumFilter);
            SaveOrder(SettingsEnum.AlbumOrder, sortEnum, ascending);

            List<int> genresId = new();
            List<int> artistsId = new();
            List<AlbumTypeEnum> albumTypes = new();

            foreach (FilterModel filter in filters)
            {
                switch (filter.FilterType)
                {
                    case FilterEnum.Artist:
                        artistsId.Add(filter.ValueId);
                        break;
                    case FilterEnum.Genre:
                        genresId.Add(filter.ValueId);
                        break;
                    case FilterEnum.AlbumType:
                        albumTypes.Add((AlbumTypeEnum)filter.ValueId);
                        break;
                    default:
                        break;
                }
            }

            return await DataAccess.Connection.SearchAlbums(genresId, artistsId, albumTypes, searchString, sortEnum, ascending);
        }

        public static async Task<List<PlaylistModel>> FilterPlaylist(string searchString)
        {
            searchString = searchString.ToLower();
            List<PlaylistModel> output = await DataAccess.Connection.GetAllPlaylists();

            output = output.Where(p => p.Name.ToLower().Contains(searchString) || p.Description.ToLower().Contains(searchString)).ToList();

            return output;
        }

        public static void SaveFilter(List<FilterModel> filters, SettingsEnum settingsEnum)
        {
            // string look like this:
            // FilterTypeInt:id.id.id/FilterTypeInt:id.id.id/
            // 3:2.4/1:42
            string filterText = "";

            if (filters is not null && filters.Count > 0)
            {
                filters = filters.OrderBy(f => (int)f.FilterType).ToList();

                FilterEnum currentFilterType = filters[0].FilterType;
                string stringFilterType = ((int)currentFilterType).ToString();
                filterText += stringFilterType + ':';
                foreach (FilterModel filter in filters)
                {
                    if (currentFilterType != filter.FilterType)
                    {
                        currentFilterType = filter.FilterType;
                        stringFilterType = ((int)currentFilterType).ToString();
                        filterText += '/' + stringFilterType + ':';
                    }

                    filterText += filter.ValueId.ToString() + '.';
                }
            }
            else
            {
                filterText = "NONE";
            }

            ConfigurationService.SetPreference(settingsEnum, filterText);
        }

        private static void SaveOrder(SettingsEnum settingsEnum, SortEnum sort, bool ascending)
        {
            string isAscending = ascending ? "1" : "0";
            ConfigurationService.SetPreference(settingsEnum, ((int)sort).ToString() + "/" + isAscending);
        }
    }
}
