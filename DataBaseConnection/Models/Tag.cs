using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using MessageControl;
using Microsoft.EntityFrameworkCore;
using MusicPlay.Database.DatabaseAccess;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models.DataBaseModels;

namespace MusicPlay.Database.Models
{
    [Table("Tag")]
    public class Tag : PlayableModel
    {
        private string _name = "";
        private List<AlbumTag> _albumTags;
        private List<ArtistTag> _artistTags;
        private List<TrackTag> _trackTags;
        private List<PlaylistTag> _playlistTags;

        public string Name
        {
            get => _name;
            set
            {
                SetField(ref _name, value);
            }
        }

        public List<AlbumTag> AlbumTags
        {
            get
            {
                if(_albumTags.IsNullOrEmpty())
                {
                    using DatabaseContext context = new();
                    _albumTags = [..context.AlbumTags.Where(at => at.TagId == Id)
                                                    .Include(at => at.Album)];
                }
                return _albumTags;
            }
            set => SetField(ref _albumTags, value);
        }

        public List<ArtistTag> ArtistTags
        {
            get
            {
                if (_artistTags.IsNullOrEmpty())
                {
                    using DatabaseContext context = new();
                    _artistTags = [.. context.ArtistTags.Where(at => at.TagId == Id)
                                                    .Include(at => at.Artist)];
                }
                return _artistTags;
            }
            set => SetField(ref _artistTags, value);
        }

        public List<TrackTag> TrackTags
        {
            get
            {
                if (_trackTags.IsNullOrEmpty())
                {
                    using DatabaseContext context = new();
                    _trackTags = [.. context.TrackTags.Where(at => at.TagId == Id)
                                                    .Include(at => at.Track)];
                }
                return _trackTags;
            }
            set => SetField(ref _trackTags, value);
        }

        public List<PlaylistTag> PlaylistTags
        {
            get
            {
                if (_playlistTags.IsNullOrEmpty())
                {
                    using DatabaseContext context = new();
                    _playlistTags = [.. context.PlaylistTags.Where(at => at.TagId == Id)
                                                    .Include(at => at.Playlist)];
                }
                return _playlistTags;
            }
            set => SetField(ref _playlistTags, value);
        }

        private ObservableCollection<Album> _albums;
        [NotMapped]
        public ObservableCollection<Album> Albums
        {
            get
            {
                if(_albums.IsNullOrEmpty())
                {
                    _albums = new(AlbumTags.Select(at => at.Album));
                }
                return _albums;
            }
            set => SetField(ref _albums, value);
        }

        private ObservableCollection<Artist> _artists;
        [NotMapped]
        public ObservableCollection<Artist> Artists
        {
            get
            {
                if (_artists.IsNullOrEmpty())
                {
                    _artists = new(ArtistTags.Select(at => at.Artist));
                }
                return _artists;
            }
            set => SetField(ref _artists, value);
        }

        private ObservableCollection<Track> _tracks;
        [NotMapped]
        public ObservableCollection<Track> Tracks
        {
            get
            {
                if (_tracks.IsNullOrEmpty())
                {
                    _tracks = new(TrackTags.Select(at => at.Track));
                }
                return _tracks;
            }
            set => SetField(ref _tracks, value);
        }

        private ObservableCollection<Playlist> _playlists;
        [NotMapped]
        public ObservableCollection<Playlist> Playlists
        {
            get
            {
                if (_playlists.IsNullOrEmpty())
                {
                    _playlists = new(PlaylistTags.Select(at => at.Playlist));
                }
                return _playlists;
            }
            set => SetField(ref _playlists, value);
        }

        public Tag(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public Tag(string name) 
        {
            Name = name;
        }

        public Tag() { }

        public static async ValueTask Insert(Tag tag)
        {
            using DatabaseContext context = new();
            context.Tags.Add(tag);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Get a tag with all its artists, albums, tracks and playlists
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static ValueTask<Tag> Get(int id)
        {
            using DatabaseContext context = new();
            return context.Tags.FindAsync(id);
        }

        /// <summary>
        /// Get all the tags without its artists, albums, tracks and playlists
        /// </summary>
        /// <returns></returns>
        public static List<Tag> GetAll()
        {
            using DatabaseContext context = new();
            return [.. context.Tags];
        }

        public static async Task Update(Tag tag, string newName)
        {
            using DatabaseContext context = new();
            if (!context.Tags.Local.Contains(tag))
            {
                context.Attach(tag);
            }

            tag.Name = newName;
            await context.SaveChangesAsync();
        }

        public static async Task Add<T>(Tag tag, T entity) where T : PlayableModel
        {
            using DatabaseContext context = new();

            if (entity is Album album)
            {
                context.AlbumTags.Add(new AlbumTag(tag.Id, album.Id));
            }
            else if (entity is Artist artist)
            {
                // need to use ids...
                context.ArtistTags.Add(new ArtistTag(tag, artist));
            }
            else if (entity is Track track)
            {
                // need to use ids...
                context.TrackTags.Add(new TrackTag(tag, track));
            }
            else if (entity is Playlist playlist)
            {
                // need to use ids...
                context.PlaylistTags.Add(new PlaylistTag(tag, playlist));
            }
            else
            {
                throw new NotSupportedException($"The type {typeof(T).Name} is not supported");
            }

            await context.SaveChangesAsync();
        }

        public static async Task Remove<T>(Tag tag, T entity) where T : TagRelationModel
        {
            using DatabaseContext context = new();

            if (!context.Tags.Local.Contains(tag))
            {
                context.Tags.Attach(tag);
            }

            if (entity is AlbumTag album)
            {
                tag.AlbumTags.Remove(album);
            }
            else if (entity is ArtistTag artist)
            {
                tag.ArtistTags.Remove(artist);
            }
            else if (entity is TrackTag track)
            {
                tag.TrackTags.Remove(track);
            }
            else if (entity is PlaylistTag playlist)
            {
                tag.PlaylistTags.Remove(playlist);
            }
            else
            {
                throw new NotSupportedException($"The type {typeof(T).Name} is not supported");
            }

            await context.SaveChangesAsync();
        }

        public static async Task Delete(Tag tag)
        {
            using DatabaseContext context = new();
            context.Tags.Remove(tag);
            await context.SaveChangesAsync();
        }
    }
}
