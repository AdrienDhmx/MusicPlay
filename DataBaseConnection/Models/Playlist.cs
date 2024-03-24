using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Controls.Primitives;
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
        private ObservableCollection<PlaylistTrack> _playlistTracks = [];
        private List<PlaylistTag> _playlistTags = [];

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

        public ObservableCollection<PlaylistTrack> PlaylistTracks
        {
            get
            {
                if (_playlistTracks.IsNullOrEmpty())
                {
                    using DatabaseContext context = new();
                    _playlistTracks = new(
                    [
                        .. context.PlaylistTracks.Where(pt => pt.PlaylistId == Id)
                                                .Include(pt => pt.Track)
                                                .ThenInclude(t => t.Album)
                                                .OrderBy(pt => pt.TrackIndex)
                    ]);
               }

                return _playlistTracks;
            }
            set => SetField(ref _playlistTracks, value);
        }

        private ObservableCollection<Track> _tracks;
        [NotMapped]
        public ObservableCollection<Track> Tracks
        {
            get
            {
                if (_tracks.IsNullOrEmpty())
                {
                    _tracks = new(PlaylistTracks.Select(pt => pt.Track));
                }

                return _tracks;
            }
        }

        public List<PlaylistTag> PlaylistTags
        {
            get
            {
                if(_playlistTags.IsNullOrEmpty())
                {
                    using DatabaseContext context = new();
                    _playlistTags = [.. context.PlaylistTags.Where(pt => pt.Playlist.Id == Id)];
                }
                return _playlistTags;
            }
            set => SetField(ref _playlistTags, value);
        }

        private ObservableCollection<Tag> _tags;
        [NotMapped]
        public ObservableCollection<Tag> Tags
        {
            get
            {
                if(_tags.IsNullOrEmpty())
                {
                    _tags = new(PlaylistTags.Select(pt => pt.Tag));
                }
                return _tags;
            }
        }

        public override int Length
        {
            get
            {
                if (_length == 0)
                {
                    _length = Tracks.GetTotalLength();

                    if(_length != 0)
                    {
                        OnPropertyChanged(nameof(Duration));
                        OnPropertyChanged(nameof(WrittenDuration));
                    }
                }
                return _length;
            }
            set
            {
                SetField(ref _length, value);
                OnPropertyChanged(nameof(Duration));
                OnPropertyChanged(nameof(WrittenDuration));
            }
        }

        public void ClearTracks()
        {
            PlaylistTracks = null;
            _tracks = null;
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

        public static async ValueTask Insert(Playlist playlist)
        {
            using DatabaseContext context = new();
            context.Playlists.Add(playlist);
            await context.SaveChangesAsync();
        }

        public static Playlist Get(int id)
        {
            using DatabaseContext context = new();
            return context.Playlists.Find(id);
        }

        public static async Task<List<Playlist>> GetAll()
        {
            using DatabaseContext context = new();
            return await context.Playlists.ToListAsync();
        }

        public static async Task<List<Playlist>> GetAllWhereNotTrack(Track track)
        {
            using DatabaseContext context = new();
            return await context.Playlists.Where(p => !p.PlaylistTracks.Select(t => t.TrackId).Contains(track.Id)).ToListAsync();
        }

        public static List<Playlist> GetLastPlayed(int top)
            => GetLastPlayed<Playlist>(top);

        public static List<Playlist> GetMostPlayed(int top)
            => GetMostPlayed<Playlist>(top);

        public static List<PlaylistTrack> GetTracks(Playlist playlist)
        {
            using DatabaseContext context = new();
            return [.. context.PlaylistTracks
                        .Where(pt => pt.PlaylistId == playlist.Id)
                        .Include(pt => pt.Track)
                        .ThenInclude(t => t.Album)
                        .OrderBy(pt => pt.TrackIndex)
                ];
        }

        private static bool CanAddTrackToPlaylist(Playlist playlist, Track track)
        {
            return !playlist.Tracks.Any(t => t.Id == track.Id);
        }

        public static void AddTrack(Playlist playlist, Track track)
        {
            if (!CanAddTrackToPlaylist(playlist, track))
            {
                return;
            }

            using DatabaseContext context = new();
            PlaylistTrack playlistTrack = new PlaylistTrack(playlist.Id, track.Id, playlist.PlaylistTracks.Count + 1);
            context.PlaylistTracks.Add(playlistTrack);
            context.SaveChanges();
            playlistTrack.Track = track;
            playlist.PlaylistTracks.Add(playlistTrack);
        }

        public static async Task InsertTrack(Playlist playlist, Track track, int index)
        {
            if(!CanAddTrackToPlaylist(playlist, track))
            {
                return;
            }

            using DatabaseContext context = new();
            PlaylistTrack playlistTrack = new PlaylistTrack(playlist.Id, track.Id, index + 1);
            context.PlaylistTracks.Add(playlistTrack);
            context.SaveChanges();

            playlist.PlaylistTracks.Insert(index, playlistTrack);
            UpdateTrackIndexes(playlist.PlaylistTracks, playlist.Id, context, 0);
            await context.SaveChangesAsync();
            playlistTrack.Track = track;
        }

        public static async Task<int> AddTracks(Playlist playlist, List<Track> tracks)
        {
            // make sure only new tracks are added
            tracks.RemoveAll(t => playlist.Tracks.Any(pt => pt.Id == t.Id));

            using DatabaseContext context = new();
            for (int i = 0; i < tracks.Count; ++i)
            {
                Track track = tracks[i];
                PlaylistTrack playlistTrack = new(playlist.Id, track.Id, playlist.PlaylistTracks.Count + i + 1);
                context.PlaylistTracks.Add(playlistTrack);
            }
            await context.SaveChangesAsync();
            return tracks.Count;
        }

        public static async Task MoveTrack(Playlist playlist, int oldIndex, int newIndex) 
        {
            using DatabaseContext context = new();
            playlist.PlaylistTracks.Move(oldIndex, newIndex);
            int startIndex = oldIndex > newIndex ? newIndex : oldIndex;

            UpdateTrackIndexes(playlist.PlaylistTracks, playlist.Id, context, startIndex);
            await context.SaveChangesAsync();
        }

        public static void UpdateTrackIndexes(ICollection<PlaylistTrack> tracks, int playlistId, DatabaseContext context, int startIndex = 0)
        {
            for (int i = startIndex; i < tracks.Count; i++)
            {
                context.PlaylistTracks.Where(pt => pt.PlaylistId == playlistId && pt.TrackId == tracks.ElementAt(i).TrackId)
                                       .ExecuteUpdate(propCall => propCall.SetProperty(pt => pt.TrackIndex, i + 1));
            }
        }

        public static async Task RemoveTrack(Playlist playlist, PlaylistTrack track)
        {
            await RemoveTrack(playlist, track.Track);
        }

        public static async Task RemoveTrack(Playlist playlist, Track track)
        {
            using DatabaseContext context = new();
            PlaylistTrack playlistTrack = context.PlaylistTracks.Find(playlist.Id, track.Id);
            context.PlaylistTracks.Remove(playlistTrack);
            playlist.PlaylistTracks.RemoveAt(playlistTrack.TrackIndex - 1);
            UpdateTrackIndexes(playlist.PlaylistTracks, playlist.Id, context, 0);
            await context.SaveChangesAsync();
        }

        public static async Task Update(Playlist playlist, string newName, string newDescription, string newCover)
        {
            using DatabaseContext context = new();
            Playlist playlistToUpdate = context.Playlists.Find(playlist.Id);
            context.Update(playlistToUpdate);
            playlistToUpdate.Name = newName;
            playlistToUpdate.Description = newDescription;
            playlistToUpdate.Cover = newCover;
            await context.SaveChangesAsync();
        }

        public static void UpdateCover(Playlist playlist, string newCover)
        {
            using DatabaseContext context = new();
            context.Update(playlist);
            playlist.Cover = newCover;
            context.SaveChanges();
        }

        public static async Task Delete(Playlist playlist)
        {
            using DatabaseContext context = new();
            context.PlaylistTracks.Where(pt => pt.PlaylistId == playlist.Id).ExecuteDelete();
            context.PlaylistTags.Where(pt => pt.PlaylistId == playlist.Id).ExecuteDelete();
            context.Playlists.Where(p => p.Id == playlist.Id).ExecuteDelete();
            await context.SaveChangesAsync();
        }
    }
}
