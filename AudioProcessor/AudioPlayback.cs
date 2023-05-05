using System.Timers;
using AudioHandler.Enums;
using AudioHandler.Models;
using ManagedBass;
using ManagedBass.Fx;
using MessageControl;
using MusicPlayModels;
using Timer = System.Timers.Timer;

namespace AudioHandler
{
    public class AudioPlayback : ObservableObject, IAudioPlayback
    {
        private readonly Timer deviceTimer;
        private string _file;
        private int _savedVolume = 100;
        private int _deviceChoosenByUser = -1;

        private int _stream = -1;
        public int Stream
        {
            get => _stream;
            private set
            {
                _stream = value;
                OnStreamChanged();
            }
        }

        public bool AutoChangeOutputDevice { get; private set; } = false;

        public DeviceModel Device { get; private set; }


        public int Frequency { get; private set; } = 44100;

        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get => _isPlaying;
            private set
            {
                SetField(ref _isPlaying, value);
                IsPlayingChanged?.Invoke(IsPlaying);
            }
        }

        private bool _isLooping = false;
        public bool IsLooping
        {
            get => _isLooping;
            private set 
            { 
                SetField(ref _isLooping, value); 
                IsLoopingChanged?.Invoke();
            }
        }

        public int VolumeStep { get; set; } = 50;
        private int _volume;
        public int Volume
        {
            get => _volume;
            set
            {
                if (value < 0)
                    _volume = 0;
                if (value > 10000)
                {
                    _volume = 10000;
                }
                else
                    _volume = value;
                SetGlobalStreamVolume();
                OnVolumeChanged();
            }
        }

        public event Action StreamChanged;
        public event Action DeviceChanged;
        public event Action<bool> IsPlayingChanged;
        public event Action IsLoopingChanged;
        public event Action<int> VolumeChanged;
        private void OnStreamChanged()
        {
            StreamChanged?.Invoke();
        }
        private void OnIsDeviceChanged()
        {
            DeviceChanged?.Invoke();
        }
        private void OnVolumeChanged()
        {
            VolumeChanged?.Invoke(Volume);
        }

        public TimeSpan Position
        {
            get
            {
                if (Stream == -1)
                    return TimeSpan.Zero;
                return TimeSpan.FromSeconds(Bass.ChannelBytes2Seconds(Stream, Bass.ChannelGetPosition(Stream, PositionFlags.Bytes)));
            }
            set
            {
                if (Stream == -1)
                    return;
                Bass.ChannelSetPosition(Stream, Bass.ChannelSeconds2Bytes(Stream, value.TotalSeconds), PositionFlags.Bytes);
            }
        }

        public AudioPlayback()
        {
            Init();

            // look for new connected devices
            deviceTimer = new Timer(2000);
        }

        public void AutoChangeOutput(bool value)
        {
            AutoChangeOutputDevice = value;
            if (AutoChangeOutputDevice)
            {
                deviceTimer.AutoReset = true;
                deviceTimer.Elapsed += DeviceTimer_Elapsed;
                deviceTimer.Start();
            }
            else
            {
                deviceTimer.Stop();
                deviceTimer.Elapsed -= DeviceTimer_Elapsed;
            }
        }

        private void ErrorHandler()
        {
            switch (Bass.LastError)
            {
                case Errors.Unknown:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: Unknown Audio Error."));
                    break;
                case Errors.FileOpen:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the audio file was already opened."));
                    break;
                case Errors.Driver:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: no valid driver found."));
                    break;
                case Errors.SampleFormat:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: unsupported file format."));
                    break;
                case Errors.Position:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: playback position invalid."));
                    break;
                case Errors.Init:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the device could not be initialized."));
                    break;
                case Errors.Already:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("The device is already initialized."));
                    break;
                case Errors.No3D:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: 3D is not supported."));
                    break;
                case Errors.Device:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the device is not supported/valid."));
                    break;
                case Errors.NoInternet:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("You are not connected to the internet."));
                    break;
                case Errors.FileFormat:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: unsupported file format."));
                    break;
                case Errors.Speaker:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: speakers are unavailable."));
                    break;
                case Errors.Codec:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: unsupported codec."));
                    break;
                case Errors.Ended:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the file or channel has ended."));
                    break;
                case Errors.Busy:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the device is busy with another process."));
                    break;
                case Errors.Unstreamable:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the file can't be streamed using the buffered file system."));
                    break;
                case Errors.WmaLicense:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the file is protected and can't be streamed."));
                    break;
                case Errors.WmaAccesDenied:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the file is protected and can't be streamed."));
                    break;
                case Errors.Wasapi:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: WASAPI is not available."));
                    break;
                case Errors.Memory:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: there has been a memory error."));
                    break;
                case Errors.BufferLost:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the sample buffer was lost."));
                    break;
                case Errors.Handle:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the file was not loaded correctly."));
                    break;
                case Errors.Parameter:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: a wrong parameter was used."));
                    break;
                case Errors.NoChannel:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: there is no free channel."));
                    break;
                case Errors.Start:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the audio engine has not been initialized successfully."));
                    break;
                case Errors.NoEAX:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: you do not have EAX (Environmental Audio Extensions) support."));
                    break;
                case Errors.NotPlaying:
                    if(!Init(AudioOutput.GetDefaultdevice().Index, Frequency))
                    {
                        MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the file is not playing, the audio device could not be initialized correctly."));
                    }
                    else if(!RetryLoadingAndPLayingFile()) // device initalized successfully
                    { 
                        MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the file is not playing and the device has successfully been initialized, the error might come from the file being corrupted."));
                    }
                    break;
                case Errors.SampleRate:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the sample rate of this file is not valid."));
                    break;
                case Errors.NotFile:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the file is not a valid audio file."));
                    break;
                case Errors.Empty:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the file was not loaded correctly or is empty."));
                    break;
                case Errors.NoFX:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the effects are not available."));
                    break;
                case Errors.Playing:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the channel is playing."));
                    break;
                case Errors.NotAvailable:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: the requested data is not available."));
                    break;
                case Errors.DirectX:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: your directx version is not supported."));
                    break;
                case Errors.Timeout:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: connection timeout, check your internet connection and try again."));
                    break;
                case Errors.Mp4NoStream:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: your mp4 file has media segment in the wrong order (\"madt\" before \"moov\")"));
                    break;
                default:
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Unknown Audio Error."));
                    break;
            }
        }

        private void DeviceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DeviceModel device = AudioOutput.GetDefaultdevice();
            // the user has not choosen a device manually
            // and new default device or the current device isn't available anymore
            if (_deviceChoosenByUser == -1 && (device.Index != Device.Index || !AudioOutput.GetAllDevices().Any(d => d.Index == Device.Index)))
            {
                if (!ChangeOutputDevice(device, false))
                {
                    ErrorHandler();
                }
            }
        }

        public bool ChangeOutputDevice(DeviceModel device, bool fromUser = true)
        {
            if (Device.Index == device.Index) return false;

            // save current stream pos and state
            TimeSpan time = Position;
            bool isPlaying = IsPlaying;

            // dipose of all resources for the current stream and device
            Dispose();

            // try init the new device
            bool initiliazed = Init(device.Index, Frequency);
            if (initiliazed)
            {
                Device = device;
                LoadAnPlay(_file);
                Play();

                // set the position
                Position = time;

                // loop if it was looping
                Loop(IsLooping);

                // if the device are speakers the playback does not resume but if the device are headphones then resume
                if ((!isPlaying && Device.Type != DeviceTypeEnum.HeadPhones) || Device.Type == DeviceTypeEnum.Speakers)
                {
                    Pause();
                }

                if (fromUser)
                {
                    _deviceChoosenByUser = device.Index;
                }
                else
                {
                    _deviceChoosenByUser = -1;
                }
                OnIsDeviceChanged();
            }
            return initiliazed;
        }

        public void Load(string file)
        {
            _file = file;
            if (Stream != -1)
            {
                // avoid some edge cases where a stream is played mutiple times simultaneously by stopping them all
                Bass.SampleStop(Stream);
                // free the stream resources (FX and DSP too if any)
                Bass.StreamFree(Stream);
            }

            Stream = Bass.CreateStream(file, Flags: BassFlags.Default);
        }

        public void Play()
        {
            if (Stream != -1)
            {
                if (Bass.ChannelPlay(Stream))
                {
                    IsPlaying = true;
                }
                else
                {
                    ErrorHandler();
                }
            }
        }

        private bool RetryLoadingAndPLayingFile()
        {
            Load(_file);
            if (Stream == -1)
                return false;

            if (Bass.ChannelPlay(Stream))
            {
                IsPlaying = true;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void LoadAnPlay(string file)
        {
            Load(file);
            Play();
        }

        public void Pause()
        {
            if (Stream != -1)
            {
                if (Bass.ChannelPause(Stream))
                {
                    IsPlaying = false;
                }
                else
                {
                    ErrorHandler();
                }
            }
        }

        public void Stop()
        {
            if(Stream != -1)
            {
                if (Bass.ChannelStop(Stream))
                {
                    IsPlaying= false;
                }
                else
                {
                    ErrorHandler();
                }
            }
        }

        public void Dispose()
        {
            Bass.Free();
            Stream = -1;
        }

        public bool Init(int device, int frequency)
        {
            return Bass.Init(device, frequency, DeviceInitFlags.Default);
        }

        public void Init()
        {
            _volume = GetCurrentVolume();
            Device = AudioOutput.GetDefaultdevice();
            if(!Bass.Init(Device.Index, Frequency, DeviceInitFlags.Default))
            {
                ErrorHandler();
            }
        }

        private void SetGlobalStreamVolume()
        {
            Bass.GlobalStreamVolume = Volume;
        }

        public void DecreaseVolume(int step)
        {
            Volume -= step;
        }

        public void DecreaseVolume()
        {
            Volume -= VolumeStep;
        }

        public void IncreaseVolume(int step)
        {
            Volume += step;
        }

        public void IncreaseVolume()
        {
            Volume += VolumeStep;
        }

        public int GetCurrentVolume()
        {
            return Bass.GlobalMusicVolume;
        }

        public void Mute()
        {
            if(Volume == 0)
            {
                Volume = _savedVolume;
            }
            else
            {
                _savedVolume = Volume;
                Volume = 0;
            }
        }

        public void Loop()
        {
            Loop(!IsLooping);
        }

        public void Loop(bool loop)
        {
            if (loop)
            {
                if (Bass.ChannelAddFlag(Stream, BassFlags.Loop))
                {
                    IsLooping = true;
                }
                else
                {
                    ErrorHandler();
                }
            }
            else
            {
                if (Bass.ChannelRemoveFlag(Stream, BassFlags.Loop))
                {
                    IsLooping = false;
                }
                else
                {
                    ErrorHandler();
                }
            }
        }
    }
}
