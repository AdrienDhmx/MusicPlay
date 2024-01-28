using MusicPlay.Database.Enums;

namespace MusicPlay.Database.Models.AudioModels
{
    public class AudioDeviceModel : BaseModel
    {
        private string _hardwareName = string.Empty;
        private string _name = string.Empty;
        private int _index = 0;
        private AudioDeviceTypeEnum _deviceType;
        private bool _isInitialized = false;
        private bool _isDefault = false;
        private bool _isDetected = false;

        /// <summary>
        /// The name of the hardware => the name it gives to the OS
        /// </summary>
        public string HardwareName
        {
            get => _hardwareName;
            init => SetField(ref _hardwareName, value);
        }

        /// <summary>
        /// The name given by the user, it's set to <see cref="HardwareName"/> by default
        /// </summary>
        public string Name
        {
            get => _name;
            set => SetField(ref _name, value);
        }

        /// <summary>
        /// The index of the hardware for the OS
        /// </summary>
        public int Index
        {
            get => _index;
            set => SetField(ref _index, value);
        }

        /// <summary>
        /// The type of output device
        /// </summary>
        public AudioDeviceTypeEnum DeviceType
        {
            get => _deviceType;
            set => SetField(ref _deviceType, value);
        }

        /// <summary>
        /// Whether the device is initialized (i.e.: the current output device)
        /// </summary>
        public bool IsInitialized
        {
            get => _isInitialized;
            set => SetField(ref _isInitialized, value);
        }

        /// <summary>
        /// Whether this device is the default output device (for the OS)
        /// </summary>
        public bool IsDefault
        {
            get => _isDefault; 
            set => SetField(ref _isDefault, value);
        }

        /// <summary>
        /// Whether the hardware is currently available (detected by the OS)
        /// </summary>
        public bool IsDetected
        {
            get => _isDetected;
            set => SetField(ref _isDetected, value);
        }

        public AudioDeviceModel(string hardwareName)
        {
            HardwareName = hardwareName;
        }
    }
}
