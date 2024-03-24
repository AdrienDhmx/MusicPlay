using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media;

using MusicPlay.Database.Models;
using MusicPlayUI.Core.Helpers;

namespace MusicPlayUI.MVVM.Models
{
    public class UITagModel : Tag
    {
        private string _cover;
        public string Cover
        {
            get
            {
                return _cover;
            }
            private set
            {
                SetField(ref _cover, value);
            }
        }

        private List<string> _multipleCovers;
        public List<string> MultipleCovers => GetMultipleCovers();

        protected int maxNumberOfCover = 14;
        public UITagModel(Tag tag, string cover) : base(tag.Id, tag.Name)
        {
            Cover = cover;
            AlbumTags = tag.AlbumTags;
            ArtistTags = tag.ArtistTags;
            PlaylistTags = tag.PlaylistTags;
            TrackTags = tag.TrackTags;
        }

        protected List<string> GetMultipleCovers()
        {
            if (_multipleCovers != null) return _multipleCovers;
            else if (ArtistTags.Count == 0 && AlbumTags.Count == 0 && PlaylistTags.Count == 0 && TrackTags.Count == 0) return new();

            List<string> covers = new();
            int totalCoverCount = 0;
            for (int coverCount = 0; totalCoverCount < maxNumberOfCover; coverCount++)
            {
                if (totalCoverCount < coverCount && totalCoverCount > 0)
                {
                    for(int i = 0; totalCoverCount < maxNumberOfCover && i < covers.Count; i++)
                    {
                        covers.Add(covers[i]);
                        totalCoverCount++;
                    }
                }

                if (coverCount < AlbumTags.Count && !covers.Contains(AlbumTags[coverCount].Album.AlbumCover))
                {
                    covers.Add(AlbumTags[coverCount].Album.AlbumCover);
                    totalCoverCount++;
                }

                if (coverCount < ArtistTags.Count && !covers.Contains(ArtistTags[coverCount].Artist.Cover))
                {
                    covers.Add(ArtistTags[coverCount].Artist.Cover);
                    totalCoverCount++;
                }

                if (coverCount < PlaylistTags.Count && !covers.Contains(PlaylistTags[coverCount].Playlist.Cover))
                {
                    covers.Add(PlaylistTags[coverCount].Playlist.Cover);
                    totalCoverCount++;
                }

                if (coverCount < TrackTags.Count)
                {
                    if (!string.IsNullOrWhiteSpace(TrackTags[coverCount].Track.Artwork) && !covers.Contains(TrackTags[coverCount].Track.Artwork))
                    {
                        covers.Add(TrackTags[coverCount].Track.Artwork);
                    }
                    else if(!covers.Contains(TrackTags[coverCount].Track.Album.AlbumCover))
                    {
                        covers.Add(TrackTags[coverCount].Track.Album.AlbumCover);
                        totalCoverCount++;
                    }
                }
            }
            return _multipleCovers = covers;
        }
    }

    public static class UIGenreModelExt
    {
        public static UITagModel ToUITagModel(this Tag tag)
        {
            string cover = "";
            if (tag.AlbumTags.Count > 0)
            {
                cover = tag.AlbumTags.Last().Album.AlbumCover;
            }
            else if (tag.ArtistTags.Count > 0)
            {
                cover = tag.ArtistTags.Last().Artist.Cover;
            }
            else if (tag.PlaylistTags.Count > 0)
            {
                cover = tag.PlaylistTags.Last().Playlist.Cover;
            }
            else if (tag.AlbumTags.Count > 0)
            {
                if (string.IsNullOrWhiteSpace(tag.TrackTags.Last().Track.Artwork))
                    cover = tag.TrackTags.Last().Track.Album.AlbumCover;
                else
                    cover = tag.TrackTags.Last().Track.Artwork;
            }

            return new(tag, cover);
        }

        public static List<UITagModel> ToUIGenreModel(this List<Tag> tags)
        {
            List<UITagModel> uiGenres = new();

            foreach (Tag tag in tags)
            {
                uiGenres.Add(tag.ToUITagModel());
            }

            return uiGenres;
        }
    }
}
