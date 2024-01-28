using System.Windows.Media;
using MusicPlay.Database.Enums;
using MusicPlay.Database.Models;
using MusicPlay.Database.Models.AudioModels;

namespace MusicPlayUI.MVVM.Models
{
    public class FullDeviceModel : BaseModel
    {
        public string Name { get; set; }

        public int Index { get; set; }

        public AudioDeviceTypeEnum Type { get; set; }

        private bool _isInitialized;
        public bool IsInitialized
        {
            get => _isInitialized;
            set
            {
                _isInitialized = value;
                OnPropertyChanged(nameof(IsInitialized));
            }
        }

        public bool IsDefault { get; set; }

        public Geometry Icon { get; set; }

        public FullDeviceModel(AudioDeviceModel device, Geometry pathGeometry)
        {
            Name = device.Name;
            Type = device.DeviceType;
            Index = device.Index;
            IsDefault = device.IsDefault;
            IsInitialized = device.IsInitialized;
            Icon = pathGeometry;
        }
    }
}
