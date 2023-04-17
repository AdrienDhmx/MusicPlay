using AudioHandler.Enums;
using AudioHandler.Models;
using MusicPlayModels;
using System.Windows.Media;

namespace MusicPlayUI.MVVM.Models
{
    public class FullDeviceModel : BaseModel
    {
        public string Name { get; set; }

        public int Index { get; set; }

        public DeviceTypeEnum Type { get; set; }

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

        public FullDeviceModel(DeviceModel device, Geometry pathGeometry)
        {
            Name = device.Name;
            Type = device.Type;
            Index = device.Index;
            IsDefault = device.IsDefault;
            IsInitialized = device.IsInitialized;
            Icon = pathGeometry;
        }
    }
}
