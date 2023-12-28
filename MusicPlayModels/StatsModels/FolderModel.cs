using MusicPlayModels;

namespace MusicPlayModels.StatsModels
{
    public class FolderModel : BaseModel
    {
        private string _name = string.Empty;
        private bool _monitored = true;
        private int _trackImportedCount = 0;
        private bool _scanning = false;

        public string Path { get; init; } // can only be set once

        public string Name 
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        public bool Monitored
        {
            get => _monitored;
            set => SetField(ref _monitored, value);
        }

        public int TrackImportedCount
        {
            get => _trackImportedCount;
            set => SetField(ref _trackImportedCount, value);
        }

        public bool Scanning
        {
            get => _scanning;
            set => SetField(ref _scanning, value);
        }

        public FolderModel(string path)
        {
            Path = path;
            Name = Path.Split('\\').Last();
        }
    }
}
