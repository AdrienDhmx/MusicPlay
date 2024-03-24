
using MusicPlay.Language;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.Core.Enums;
using MusicPlay.Database.Enums;
using MusicPlay.Database.Models;
using MusicPlay.Database.Models.DataBaseModels;
using MusicPlay.Database.DatabaseAccess;

namespace MusicPlayUI.Core.Factories
{
    public static class FilterFactory
    {
        public static List<FilterModel> GetPrimaryArtistFilter()
        {
            List<FilterModel> filters = [];
            using DatabaseContext context = new();
            List<Artist> albumArtists = [.. context.Artists.Where(a => a.ArtistRoles.Any(ar => ar.RoleId == 1))];
            foreach (Artist artist in albumArtists)
            {
                FilterModel filter = new FilterModel(artist.Id, artist.Name, FilterEnum.Artist);
                filters.Add(filter);
            }

            return filters;
        }

        public static List<FilterModel> GetGenreFilter()
        {
            List<FilterModel> filters = new();
            List<Tag> allGenre = Tag.GetAll();

            foreach (Tag genre in allGenre.OrderBy(g => g.Name))
            {
                FilterModel filter = new(genre.Id, genre.Name, FilterEnum.Tag);
                filters.Add(filter);
            }

            return filters;
        }

        public static List<FilterModel> GetAlbumTypeFilter()
        {
            List<FilterModel> filters = new()
            {
                new((int)AlbumTypeEnum.Main,"Main", FilterEnum.AlbumType),
                new((int)AlbumTypeEnum.EP,"EP", FilterEnum.AlbumType),
                new( (int)AlbumTypeEnum.Single,"Singles", FilterEnum.AlbumType),
            };

            return filters;
        }


        public static List<FilterModel> GetArtistRoleFilter()
        {
            List<FilterModel> filters = [];
            foreach(Role role in Role.GetAll())
            {
                filters.Add(new(role.Id, role.Name, FilterEnum.ArtistType));
            }

            return filters;
        }
    }
}
