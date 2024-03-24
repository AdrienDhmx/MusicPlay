using MusicPlayModels.Enums;
using MusicPlayModels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels.MusicModels
{
    public class PlaylistModel : PlayableModel, ITaggable
    {
        private string _name = "";
        private string _description = "";
        private string _cover = "";
        private PlaylistTypeEnum _playlistType = PlaylistTypeEnum.UserPlaylist;
        private List<OrderedTrackModel> _tracks = new();
        private List<TagModel> _tags = new();

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

        public PlaylistTypeEnum PlaylistType
        {
            get => _playlistType;
            set => SetField(ref _playlistType, value);
        }

        public List<OrderedTrackModel> Tracks
        {
            get => _tracks;
            set => SetField(ref _tracks, value);
        }

        public List<TagModel> Tags
        {
            get => _tags;
            set => SetField(ref _tags, value);
        }

        public PlaylistModel(int id, string name, string description, string cover, string duration)
        {
            Id = id;
            Name = name;
            Description = description;
            Cover = cover;
            Duration = duration;
        }

        public PlaylistModel(int id, string name, string description, string cover, string duration, DateTime creationDate, DateTime updateDate, List<OrderedTrackModel> tracks)
        {
            Id = id;
            Name = name;
            Description = description;
            Cover = cover;
            Duration = duration;
            CreationDate = creationDate;
            UpdateDate = updateDate;
            Tracks = tracks;
        }

        public PlaylistModel()
        {

        }

        public override Dictionary<string, object> CreateTable()
        {
            Dictionary<string, object> keyValues = new Dictionary<string, object>
            {
                { nameof(Name), Name },
                { nameof(Description), Description },
                { nameof(Cover), Cover },
            };
            return base.CreateTable().AddRange(keyValues);
        }
    }
}
