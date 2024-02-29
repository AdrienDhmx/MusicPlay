
using LastFmNamespace.Models;
using Microsoft.EntityFrameworkCore;
using MusicPlay.Database.DatabaseAccess;
using MusicPlay.Database.Enums;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services;
using MusicPlayUI.MVVM.Models;
using Newtonsoft.Json;
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
        public static LibraryFilters GetSelectedFilters(bool album = true)
        {            
            SettingsEnum settingsEnum = album ? SettingsEnum.AlbumFilter : SettingsEnum.ArtistFilter;
            string json = ConfigurationService.GetStringPreference(settingsEnum);
            if (json.IsNotNullOrWhiteSpace() || json == "NONE")
            {
                return new();
            }

            LibraryFilters filters = JsonConvert.DeserializeObject<LibraryFilters>(json);

            if(filters is null)
            {
                return new();
            }
            return filters;
        }

        public static (List<int>, List<int>) SeparateNevagtiveAndPositiveFilters(this ObservableCollection<FilterModel> filters)
        {
            List<int> negativeFilters = [];
            List<int> positiveFilters = [];

            foreach (FilterModel filter in filters)
            {
                if (filter.IsNegative)
                    negativeFilters.Add(filter.Id);
                else 
                    positiveFilters.Add(filter.Id);
            }
            return (negativeFilters, positiveFilters);
        }

        public static List<Artist> SortArtists(this List<Artist> artists, SortModel sortBy)
        {
            bool ascending = sortBy.IsAscending;
            IOrderedEnumerable<Artist> sortedArtists = sortBy.Type switch
            {
                SortEnum.AZ => ascending ? artists.OrderBy(a => a.Name) : artists.OrderByDescending(a => a.Name),
                SortEnum.LastPlayed => ascending ? artists.OrderBy(a => a.LastPlayed) : artists.OrderByDescending(a => a.LastPlayed),
                SortEnum.MostPlayed => ascending ? artists.OrderBy(a => a.PlayCount) : artists.OrderByDescending(a => a.PlayCount),
                SortEnum.AddedDate => ascending ? artists.OrderBy(a => a.CreationDate) : artists.OrderByDescending(a => a.CreationDate),
                SortEnum.UpdatedDate => ascending ? artists.OrderBy(a => a.UpdateDate) : artists.OrderByDescending(a => a.UpdateDate),
                _ => artists.OrderBy(a => a.Name),
            };
            return new(sortedArtists);
        }

        public static List<Artist> FilterArtists(LibraryFilters filters, string searchString, SortModel sortBy)
        {
            using DatabaseContext context = new DatabaseContext();

            var query = context.Artists.AsQueryable();

            if (filters.Filters.IsNotNullOrEmpty())
            {

                (List<int> negativeTagIds, List<int> positiveTagIds) = SeparateNevagtiveAndPositiveFilters(filters.TagFilters);
                (List<int> negativeRoleIds, List<int> positiveRoleIds) = SeparateNevagtiveAndPositiveFilters(filters.ArtistRoleFilters);

                if (negativeTagIds.Count > 0)
                    query = query.Where(a => a.ArtistTags.Count == 0 || a.ArtistTags.Any(at => negativeTagIds.Any(t => at.TagId != t)));
                if (positiveTagIds.Count > 0)
                    query = query.Where(a => a.ArtistTags.Any(at => positiveTagIds.Any(t => at.TagId == t)));

                if (negativeRoleIds.Count > 0)
                    query = query.Where(a => a.ArtistRoles.Any(ar => negativeRoleIds.Any(t => ar.RoleId != t)));
                if (positiveRoleIds.Count > 0)
                    query = query.Where(a => a.ArtistRoles.Any(ar => positiveRoleIds.Any(t => ar.RoleId == t)));
            }

            if (searchString.IsNotNullOrWhiteSpace())
            {
                query = query.Where(a => a.Name.ToLower().Contains(searchString.ToLower())
                                || a.Albums.Any(al => al.Name.ToLower().StartsWith(searchString.ToLower())));
            }

            query = sortBy.Type switch
            {
                SortEnum.AZ => (sortBy.IsAscending ? query.OrderBy(a => a.Name) : query.OrderByDescending(a => a.Name)),
                SortEnum.LastPlayed => (sortBy.IsAscending ? query.OrderBy(a => a.LastPlayed) : query.OrderByDescending(a => a.LastPlayed)),
                SortEnum.MostPlayed => (sortBy.IsAscending ? query.OrderBy(a => a.PlayCount) : query.OrderByDescending(a => a.PlayCount)),
                SortEnum.AddedDate => (sortBy.IsAscending ? query.OrderBy(a => a.CreationDate) : query.OrderByDescending(a => a.CreationDate)),
                SortEnum.UpdatedDate => (sortBy.IsAscending ? query.OrderBy(a => a.UpdateDate) : query.OrderByDescending(a => a.UpdateDate)),
                _ => query,
            };
            return [.. query];
        }

        public static List<Album> SortAlbums(this List<Album> albums, SortModel sortBy)
        {
            bool ascending = sortBy.IsAscending;
            IOrderedEnumerable<Album> sortedAlbums = sortBy.Type switch
            {
                SortEnum.AZ => ascending ? albums.OrderBy(a => a.Name) : albums.OrderByDescending(a => a.Name),
                SortEnum.Year => ascending ? albums.OrderBy(a => a.ReleaseDate) : albums.OrderByDescending(a => a.ReleaseDate),
                SortEnum.LastPlayed => ascending ? albums.OrderBy(a => a.LastPlayed) : albums.OrderByDescending(a => a.LastPlayed),
                SortEnum.MostPlayed => ascending ? albums.OrderBy(a => a.PlayCount) : albums.OrderByDescending(a => a.PlayCount),
                SortEnum.AddedDate => ascending ? albums.OrderBy(a => a.CreationDate) : albums.OrderByDescending(a => a.CreationDate),
                SortEnum.UpdatedDate => ascending ? albums.OrderBy(a => a.UpdateDate) : albums.OrderByDescending(a => a.UpdateDate),
                _ => albums.OrderBy(a => a.Name),
            };
            return new(sortedAlbums);
        }

        public static List<Album> FilterAlbum(LibraryFilters filters, string searchString, SortModel sortBy)
        {
            using DatabaseContext context = new DatabaseContext();

            var query = context.Albums.AsQueryable();

            if (!filters.Filters.IsNullOrEmpty())
            {
                (List<int> negativeTagIds, List<int> positiveTagIds) = SeparateNevagtiveAndPositiveFilters(filters.TagFilters);
                (List<int> negativeArtistIds, List<int> positiveArtistIds) = SeparateNevagtiveAndPositiveFilters(filters.PrimaryArtistFilters);
                (List<int> negativeAlbumTypeIds, List<int> positiveAlbumTypeIds) = SeparateNevagtiveAndPositiveFilters(filters.AlbumTypeFilters);

                if (negativeTagIds.Count > 0)
                    query = query.Where(a => a.AlbumTags.Count == 0 || a.AlbumTags.Any(at => negativeTagIds.Any(t => at.TagId != t)));
                if (positiveTagIds.Count > 0)
                    query = query.Where(a => a.AlbumTags.Any(at => positiveTagIds.Any(t => at.TagId == t)));

                if (negativeArtistIds.Count > 0)
                    query = query.Where(a => negativeArtistIds.Any(artistId => artistId != a.PrimaryArtistId));
                if (positiveArtistIds.Count > 0)
                    query = query.Where(a => positiveArtistIds.Any(artistId => artistId == a.PrimaryArtistId));

                if (negativeAlbumTypeIds.Count > 0)
                    query = query.Where(a => negativeAlbumTypeIds.Any(type => type != (int)a.Type));
                if (positiveAlbumTypeIds.Count > 0)
                    query = query.Where(a => positiveAlbumTypeIds.Any(type => type == (int)a.Type));
            }

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                query = query.Where(a => a.Name.ToLower().Contains(searchString.ToLower())
                                || a.PrimaryArtist.Name.ToLower().StartsWith(searchString.ToLower())
                                || a.Tracks.Any(t => t.TrackArtistRole.Any(tar => tar.ArtistRole.Artist.Name.ToLower().StartsWith(searchString.ToLower()))));
            }

            query = sortBy.Type switch
            {
                SortEnum.AZ => (sortBy.IsAscending ? query.OrderBy(a => a.Name) : query.OrderByDescending(a => a.Name)),
                SortEnum.Year => (sortBy.IsAscending ? query.OrderBy(a => a.ReleaseDate) : query.OrderByDescending(a => a.ReleaseDate)),
                SortEnum.LastPlayed => (sortBy.IsAscending ? query.OrderBy(a => a.LastPlayed) : query.OrderByDescending(a => a.LastPlayed)),
                SortEnum.MostPlayed => (sortBy.IsAscending ? query.OrderBy(a => a.PlayCount) : query.OrderByDescending(a => a.PlayCount)),
                SortEnum.AddedDate => (sortBy.IsAscending ? query.OrderBy(a => a.CreationDate) : query.OrderByDescending(a => a.CreationDate)),
                SortEnum.UpdatedDate => (sortBy.IsAscending ? query.OrderBy(a => a.UpdateDate) : query.OrderByDescending(a => a.UpdateDate)),
                _ => query,
            };
            return [.. query.Include(a => a.PrimaryArtist)];
        }

        public static async Task<List<Playlist>> FilterPlaylist(string searchString)
        {
            searchString = searchString.ToLower();
            List<Playlist> output = await Playlist.GetAll();

            output = output.Where(p => p.Name.Contains(searchString, StringComparison.CurrentCultureIgnoreCase) || p.Description.Contains(searchString, StringComparison.CurrentCultureIgnoreCase)).ToList();

            return output;
        }

        public static void SaveFilter(SettingsEnum settingsEnum, LibraryFilters filters)
        {
            if (filters is null)
                return;

            string json = JsonConvert.SerializeObject(filters);
            ConfigurationService.SetPreference(settingsEnum, json);
        }

        public static void SaveOrder(SettingsEnum settingsEnum, SortModel sortBy)
        {
            string isAscending = sortBy.IsAscending ? "1" : "0";
            ConfigurationService.SetPreference(settingsEnum, ((int)sortBy.Type).ToString() + "/" + isAscending);
        }
    }
}
