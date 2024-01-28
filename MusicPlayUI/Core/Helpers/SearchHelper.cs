
using Microsoft.EntityFrameworkCore;
using MusicPlay.Database.DatabaseAccess;
using MusicPlay.Database.Enums;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models;
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
using TagLib;
using TagLib.Matroska;

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

        public static List<Artist> FilterArtists(List<FilterModel> filters, string searchString, SortEnum sortEnum, bool ascending = false)
        {
            SaveFilter(filters, SettingsEnum.ArtistFilter);
            SaveOrder(SettingsEnum.ArtistOrder, sortEnum, ascending);

            if (filters.IsNullOrEmpty() && searchString.IsNullOrWhiteSpace())
            {
                return Artist.GetAll();
            }

            DatabaseContext context = new DatabaseContext();

            var query = context.Artists.AsQueryable();

            if (filters.IsNotNullOrEmpty())
            {
                List<int> artistRoles = new();
                List<int> tags = new();

                foreach (FilterModel filter in filters)
                {
                    switch (filter.FilterType)
                    {
                        case FilterEnum.ArtistType:
                            artistRoles.Add(filter.ValueId);
                            break;
                        case FilterEnum.Genre:
                            tags.Add(filter.ValueId);
                            break;
                        default:
                            break;
                    }
                }

                if (artistRoles.Count != 0)
                {
                    query = query.Where(a => a.ArtistRoles.Any(ar => artistRoles.Any(r => r == ar.Role.Id)));
                }

                if (tags.Count != 0)
                {
                    query = query.Where(a => a.ArtistTags.Any(at => tags.Any(t => t == at.TagId)));
                }
            }

            if (searchString.IsNotNullOrWhiteSpace())
            {
                query = query.Where(a => a.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)
                                || a.Albums.Any(al => al.Name.StartsWith(searchString, StringComparison.CurrentCultureIgnoreCase)));
            }

            query = sortEnum switch
            {
                SortEnum.AZ => (ascending ? query.OrderBy(a => a.Name) : query.OrderByDescending(a => a.Name)),
                SortEnum.LastPlayed => (ascending ? query.OrderBy(a => a.LastPlayed) : query.OrderByDescending(a => a.LastPlayed)),
                SortEnum.MostPlayed => (ascending ? query.OrderBy(a => a.PlayCount) : query.OrderByDescending(a => a.PlayCount)),
                SortEnum.AddedDate => (ascending ? query.OrderBy(a => a.CreationDate) : query.OrderByDescending(a => a.CreationDate)),
                SortEnum.UpdatedDate => (ascending ? query.OrderBy(a => a.UpdateDate) : query.OrderByDescending(a => a.UpdateDate)),
                _ => query,
            };
            return [.. query];
        }

        public static List<Album> FilterAlbum(List<FilterModel> filters, string searchString, SortEnum sortEnum, bool ascending = false)
        {
            SaveFilter(filters, SettingsEnum.AlbumFilter);
            SaveOrder(SettingsEnum.AlbumOrder, sortEnum, ascending);

            if (filters.IsNullOrEmpty() && searchString.IsNullOrWhiteSpace())
            {
                return Album.GetAll();
            }

            DatabaseContext context = new DatabaseContext();

            var query = context.Albums.AsQueryable();

            if (!filters.IsNullOrEmpty())
            {
                List<int> tags = new();
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
                            tags.Add(filter.ValueId);
                            break;
                        case FilterEnum.AlbumType:
                            albumTypes.Add((AlbumTypeEnum)filter.ValueId);
                            break;
                        default:
                            break;
                    }
                }

                if (artistsId.Count != 0)
                {
                    query = query.Where(a => artistsId.Any(ar => ar == a.PrimaryArtist.Id));
                }

                if (tags.Count != 0)
                {
                    query = query.Where(a => a.AlbumTags.Any(at => tags.Any(g => g == at.TagId)));
                }

                if (albumTypes.Count != 0)
                {
                    query = query.Where(a => albumTypes.Any(t => t == a.Type));
                }
            }

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(a => a.Name.ToLower().Contains(searchString.ToLower())
                                || a.PrimaryArtist.Name.ToLower().StartsWith(searchString.ToLower()));
            }

            query = sortEnum switch
            {
                SortEnum.AZ => (ascending ? query.OrderBy(a => a.Name) : query.OrderByDescending(a => a.Name)),
                SortEnum.Year => (ascending ? query.OrderBy(a => a.ReleaseDate) : query.OrderByDescending(a => a.ReleaseDate)),
                SortEnum.LastPlayed => (ascending ? query.OrderBy(a => a.LastPlayed) : query.OrderByDescending(a => a.LastPlayed)),
                SortEnum.MostPlayed => (ascending ? query.OrderBy(a => a.PlayCount) : query.OrderByDescending(a => a.PlayCount)),
                SortEnum.AddedDate => (ascending ? query.OrderBy(a => a.CreationDate) : query.OrderByDescending(a => a.CreationDate)),
                SortEnum.UpdatedDate => (ascending ? query.OrderBy(a => a.UpdateDate) : query.OrderByDescending(a => a.UpdateDate)),
                _ => query,
            };
            return query.Include(a => a.PrimaryArtist).ToList();
        }

        public static async Task<List<Playlist>> FilterPlaylist(string searchString)
        {
            searchString = searchString.ToLower();
            List<Playlist> output = await Playlist.GetAll();

            output = output.Where(p => p.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase) || p.Description.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();

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
