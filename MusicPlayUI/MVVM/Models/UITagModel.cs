using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media;
using DataBaseConnection.DataAccess;
using MusicPlayModels.MusicModels;
using MusicPlayUI.Core.Helpers;

namespace MusicPlayUI.MVVM.Models
{
    public class UITagModel : TagModel
    {
        public List<AlbumModel> Albums { get; private set; }
        public List<ArtistModel> Artists { get; private set; }
        public List<PlaylistModel> Playlists { get; private set; }
        public List<TrackModel> Tracks { get; private set; }

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
        public UITagModel(TagModel genre, List<AlbumModel> albums, List<ArtistModel> artists, List<PlaylistModel> playlists, List<TrackModel> tracks, string cover) : base(genre.Id, genre.Name)
        {
            Albums = albums;
            Artists = artists;
            Cover = cover;
            Playlists = playlists;
            Tracks = tracks;
        }

        protected List<string> GetMultipleCovers()
        {
            if (_multipleCovers != null) return _multipleCovers;
            else if (Artists.Count == 0 && Albums.Count == 0 && Playlists.Count == 0 && Tracks.Count == 0) return new();

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

                if (coverCount < Albums.Count && !covers.Contains(Albums[coverCount].AlbumCover))
                {
                    covers.Add(Albums[coverCount].AlbumCover);
                    totalCoverCount++;
                }

                if (coverCount < Artists.Count && !covers.Contains(Artists[coverCount].Cover))
                {
                    covers.Add(Artists[coverCount].Cover);
                    totalCoverCount++;
                }

                if (coverCount < Playlists.Count && !covers.Contains(Playlists[coverCount].Cover))
                {
                    covers.Add(Playlists[coverCount].Cover);
                    totalCoverCount++;
                }

                if (coverCount < Tracks.Count)
                {
                    if (!string.IsNullOrWhiteSpace(Tracks[coverCount].Artwork) && !covers.Contains(Tracks[coverCount].Artwork))
                    {
                        covers.Add(Tracks[coverCount].Artwork);
                    }
                    else if(!covers.Contains(Tracks[coverCount].AlbumCover))
                    {
                        covers.Add(Tracks[coverCount].AlbumCover);
                        totalCoverCount++;
                    }
                }
            }
            return _multipleCovers = covers;
        }
    }

    public static class UIGEnreModelExt
    {
        public static async Task<List<UITagModel>> ToUIGenreModel(this List<TagModel> tags)
        {
            List<UITagModel> uiGenres = new();

            foreach (TagModel tag in tags)
            {
                List<AlbumModel> albums = await DataAccess.Connection.GetAlbumFromTag(tag.Id);
                List<ArtistModel> artists = await DataAccess.Connection.GetArtistFromTag(tag.Id);
                List<PlaylistModel> playlists = await DataAccess.Connection.GetPlaylistFromTag(tag.Id);
                List<TrackModel> tracks = await DataAccess.Connection.GetTrackFromTag(tag.Id);
                tracks = await tracks.GetAlbumTrackProperties();

                string cover = "";
                if(albums.Count > 0)
                {
                    cover = albums.Last().AlbumCover;
                }
                else if (artists.Count > 0)
                {
                    cover = artists.Last().Cover;
                }
                else if (playlists.Count > 0)
                {
                    cover = playlists.Last().Cover;
                }
                else if (tracks.Count > 0)
                {
                    if(string.IsNullOrWhiteSpace(tracks.Last().Artwork))
                        cover = tracks.Last().AlbumCover;
                    else 
                        cover = tracks.Last().Artwork;
                }

                uiGenres.Add(new(tag, albums, artists, playlists, tracks, cover));
            }

            return uiGenres;
        }
    }
}
