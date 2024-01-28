using System;
using System.Collections.Generic;
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
            get => _albumTags;
            set => SetField(ref _albumTags, value);
        }

        public List<ArtistTag> ArtistTags
        {
            get => _artistTags;
            set => SetField(ref _artistTags, value);
        }

        public List<TrackTag> TrackTags
        {
            get => _trackTags;
            set => SetField(ref _trackTags, value);
        }

        public List<PlaylistTag> PlaylistTags
        {
            get => _playlistTags;
            set => SetField(ref _playlistTags, value);
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
