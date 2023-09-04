using DataBaseConnection.DataAccess;
using MusicPlayModels.MusicModels;
using MusicPlay.Language;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.Core.Enums;
using MusicPlayModels.Enums;

namespace MusicPlayUI.Core.Factories
{
    public static class FilterFactory
    {
        public static async Task<List<FilterModel>> GetAlbumArtistFilter()
        {
            List<FilterModel> filters = new();

            List<ArtistModel> albumArtists = await DataAccess.Connection.GetAllArtists();
            albumArtists = albumArtists.Where(a => a.IsAlbumArtist).OrderBy(a => a.Name).ToList(); // only albums Artists

            foreach (ArtistModel artist in albumArtists)
            {
                FilterModel filter = new(artist.Name, artist.Id, FilterEnum.Artist);
                filters.Add(filter);
            }

            return filters;
        }

        public static async Task<List<FilterModel>> GetGenreFilter()
        {
            List<FilterModel> filters = new();
            List<TagModel> allGenre = await DataAccess.Connection.GetAllTags();

            foreach (TagModel genre in allGenre.OrderBy(g => g.Name))
            {
                FilterModel filter = new(genre.Name, genre.Id, FilterEnum.Genre);
                filters.Add(filter);
            }

            return filters;
        }

        public static List<FilterModel> GetAlbumTypeFilter()
        {
            List<FilterModel> filters = new()
            {
                new("Singles", (int)AlbumTypeEnum.Single, FilterEnum.AlbumType),
                new("EP", (int)AlbumTypeEnum.EP, FilterEnum.AlbumType),
                new("Compilation", (int)AlbumTypeEnum.Compilation, FilterEnum.AlbumType),
            };

            return filters;
        }


        public static List<FilterModel> GetArtistTypeFilter()
        {
            List<FilterModel> filters = new()
            {
                new(Resources.Album_Artists, (int)ArtistTypeEnum.AlbumArtist, FilterEnum.ArtistType),
                new(Resources.Performers, (int)ArtistTypeEnum.Performer, FilterEnum.ArtistType),
                new(Resources.Composers, (int)ArtistTypeEnum.Composer, FilterEnum.ArtistType),
                new("Lyricist", (int)ArtistTypeEnum.Lyricist, FilterEnum.ArtistType),
                new("Featured", (int)ArtistTypeEnum.Featured, FilterEnum.ArtistType),
            };

            return filters;
        }
    }
}
