using MusicPlayModels.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels.MusicModels
{
    public class PlaylistModel : BaseModel
    {
        private string _name = "";
        private string _description = "";
        private string _cover = "";

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
        public string Duration { get; set; } = "";
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public DateTime UpdateDate { get; set; } = DateTime.Now;
        public PlaylistTypeEnum PlaylistType { get; set; } = PlaylistTypeEnum.UserPlaylist;

        public List<OrderedTrackModel> Tracks { get; set; } = new();

        public List<TagModel> Tags { get; set; } = new();

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
    }
}
