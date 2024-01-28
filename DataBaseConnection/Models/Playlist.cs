using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MusicPlay.Database.DatabaseAccess;
using MusicPlay.Database.Enums;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models.DataBaseModels;

namespace MusicPlay.Database.Models
{
    [Table("Playlist")]
    public class Playlist : PlayableModel
    {
        private string _name = "";
        private string _description = "";
        private string _cover = "";
        private PlaylistTypeEnum _playlistType = PlaylistTypeEnum.UserPlaylist;
        private ObservableCollection<PlaylistTrack> _tracks = [];
        private ObservableCollection<PlaylistTag> _playlistTags = [];

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public string Cover
        {
            get { return _cover; }
            set
            {
                _cover = value;
                OnPropertyChanged(nameof(Cover));
            }
        }

        [NotMapped]
        public PlaylistTypeEnum PlaylistType
        {
            get => _playlistType;
            set => SetField(ref _playlistType, value);
        }

        public ObservableCollection<PlaylistTrack> Tracks
        {
            get
            {
                if (_tracks.IsNullOrEmpty())
                {
                    if (Tracks.IsNullOrEmpty())
                    {

                        _tracks = new(GetTracks(this));
                    }
                }

                return _tracks;
            }
            set => SetField(ref _tracks, value);
        }

        public ObservableCollection<PlaylistTag> PlaylistTags
        {
            get => _playlistTags;
            set => SetField(ref _playlistTags, value);
        }

        public Playlist(int id, string name, string description, string cover, string duration)
        {
            Id = id;
            Name = name;
            Description = description;
            Cover = cover;
        }

        public Playlist(int id, string name, string description, string cover, string duration, DateTime creationDate, DateTime updateDate)
        {
            Id = id;
            Name = name;
            Description = description;
            Cover = cover;
            CreationDate = creationDate;
            UpdateDate = updateDate;
        }

        public Playlist() { }

        public static async ValueTask Insert(Playlist playlist, bool insertRoles = true)
        {
            using DatabaseContext context = new();
            context.Playlists.Add(playlist);
            await context.SaveChangesAsync();
        }

        public static async ValueTask<Playlist?> Get(int id)
        {
            using DatabaseContext context = new();
            return await context.Playlists.FindAsync(id);
        }

        public static async Task<List<Playlist>> GetAll()
        {
            using DatabaseContext context = new();
            return await context.Playlists
                        .Include(p => p.Tracks)
                        .ToListAsync();
        }

        public static List<Playlist> GetLastPlayed(int top)
            => GetLastPlayed<Playlist>(top);

        public static async Task<List<Playlist>> GetMostPlayed(int top)
            => await GetMostPlayed<Playlist>(top)
                    .Include(p => p.Tracks)
                    .ToListAsync();

        public static List<PlaylistTrack> GetTracks(Playlist playlist)
        {
            using DatabaseContext context = new();
            return [.. context.PlaylistTracks
                        .Where(pt => pt.PlaylistId == playlist.Id)
                        .Include(pt => pt.Track)
                        .ThenInclude(t => t.Album)
                ];
        }

        public static async Task AddTrack(Playlist playlist, Track track)
        {
            using DatabaseContext context = new();
            if (!context.Tracks.Local.Any(e => e.Id == track.Id))
            {
                context.Attach(track);
            }

            playlist.Tracks.Add(new PlaylistTrack(playlist, track, playlist.Tracks.Count));
            await context.SaveChangesAsync();
        }

        public static async Task InsertTrack(Playlist playlist, Track track, int index)
        {
            using DatabaseContext context = new();
            if (!context.Tracks.Local.Any(e => e.Id == track.Id))
            {
                context.Attach(track);
            }

            playlist.Tracks.Insert(index, new PlaylistTrack(playlist, track, index));
            UpdateTrackIndexes(playlist.Tracks);
            await context.SaveChangesAsync();
        }

        public static async Task AddTracks(Playlist playlist, List<Track> tracks)
        {
            using DatabaseContext context = new();
            foreach (var track in tracks)
            {
                playlist.Tracks.Add(new PlaylistTrack(playlist, track, playlist.Tracks.Count));
            }
            await context.SaveChangesAsync();
        }

        public static async Task MoveTrack(Playlist playlist, int oldIndex, int newIndex) 
        {
            using DatabaseContext context = new();
            playlist.Tracks.Move(oldIndex, newIndex);
            UpdateTrackIndexes(playlist.Tracks);
            await context.SaveChangesAsync();
        }

        private static void UpdateTrackIndexes(ICollection<PlaylistTrack> tracks)
        {
            for (int i = 0; i < tracks.Count; i++)
            {
                tracks.ElementAt(i).TrackIndex = i;
            }
        }

        public static async Task RemoveTrack(Playlist playlist, PlaylistTrack track)
        {
            using DatabaseContext context = new();
            playlist.Tracks.Remove(track);
            await context.SaveChangesAsync();
        }

        public static async Task Update(Playlist playlist, string newName, string newDescription, string newCover)
        {
            using DatabaseContext context = new();
            playlist.Name = newName;
            playlist.Description = newDescription;
            playlist.Cover = newCover;
            await context.SaveChangesAsync();
        }

        public static async Task UpdateCover(Playlist playlist, string newCover)
        {
            using DatabaseContext context = new();
            playlist.Cover = newCover;
            await context.SaveChangesAsync();
        }

        public static async Task Delete(Playlist playlist)
        {
            using DatabaseContext context = new();
            context.Playlists.Remove(playlist);
            await context.SaveChangesAsync();
        }
    }
}
