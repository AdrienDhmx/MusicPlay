using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels.MusicModels
{
    public abstract class ArtistsRelation : PlayableModel
    {
        public List<ArtistDataRelation> Artists { get; set; } = new();

        protected ArtistsRelation(List<ArtistDataRelation> artists)
        {
            Artists = artists;
        }

        public ArtistsRelation() { }

        public bool ContainsArtist(int id)
        {
            return Artists.Any(a => a.ArtistId == id);
        }

        public bool IsAlbumArtist(int id)
        {
            return Artists.Any(a => a.IsAlbumArtist && a.ArtistId == id);
        }

        public bool IsFeatured(int id, bool exludeMainArtist = false)
        {
            if (exludeMainArtist)
                return Artists.Any(a => a.IsFeatured && !a.IsAlbumArtist && a.ArtistId == id);
            return Artists.Any(a => a.IsFeatured && a.ArtistId == id);
        }

        public bool IsComposer(int id, bool exludeMainArtist = false)
        {
            if (exludeMainArtist)
                return Artists.Any(a => a.IsComposer && !a.IsAlbumArtist && a.ArtistId == id);
            return Artists.Any(a => a.IsComposer && a.ArtistId == id);
        }

        public bool IsPerformer(int id, bool exludeMainArtist = false)
        {
            if (exludeMainArtist)
                return Artists.Any(a => a.IsPerformer && !a.IsAlbumArtist && a.ArtistId == id);
            return Artists.Any(a => a.IsPerformer && a.ArtistId == id);
        }

        public bool IsLyricist(int id, bool exludeMainArtist = false)
        {
            if (exludeMainArtist)
                return Artists.Any(a => a.IsLyricist && !a.IsAlbumArtist && a.ArtistId == id);
            return Artists.Any(a => a.IsLyricist && a.ArtistId == id);
        }

        public ArtistDataRelation? GetAlbumArtist()
        {
            ArtistDataRelation? result = Artists?.Find(a => a.IsAlbumArtist);
            if (result == null || result.ArtistId == -1)
            {
                return GetPerformer();
            }
            return result;
        }

        public ArtistDataRelation? GetFeaturedArtist()
        {
            ArtistDataRelation? result = Artists?.Find(a => a.IsFeatured && !a.IsAlbumArtist);
            if (result == null || result.ArtistId == -1)
            {
                result = Artists?.Find(a => a.IsPerformer && !a.IsAlbumArtist);
                if (result == null || result.ArtistId == -1)
                {
                    return Artists?.Find(a => a.IsComposer && !a.IsAlbumArtist);
                }
                return result;
            }
            return result;
        }

        public ArtistDataRelation? GetComposer()
        {
            ArtistDataRelation? result = Artists?.Find(a => a.IsComposer);
            if (result != null && result.ArtistId == -1)
            {
                return null;
            }
            return result;
        }

        public ArtistDataRelation? GetPerformer()
        {
            ArtistDataRelation? result = Artists?.Find(a => a.IsPerformer);
            if (result != null && result.ArtistId == -1)
            {
                return null;
            }
            return result;
        }

    }
}
