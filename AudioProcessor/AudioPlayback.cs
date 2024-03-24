using System.Timers;
using ManagedBass;
using MessageControl;
using MusicPlay.Database.Enums;
using MusicPlay.Database.Models;
using MusicPlay.Database.Models.AudioModels;
using Timer = System.Timers.Timer;

namespace AudioHandler
{
    public class AudioPlayback : ObservableObject, IAudioPlayback
    {
        public EQManager EQManager { get; set; } = new();

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
                // pass the new stream to the equalizer
                EQManager.ApplyPreset(Stream);
                OnStreamChanged();
            }
        }

        public bool AutoChangeOutputDevice { get; private set; } = false;

        public AudioDeviceModel Device { get; private set; }


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
            Errors bassError = Bass.LastError;

            string errorMessage = bassError switch
            {
                Errors.NotPlaying => HandleNotPlayingError(),
                Errors.FileOpen => "Error: the audio file was already opened.",
                Errors.Driver => "Error: no valid driver found.",
                Errors.SampleFormat => "Error: unsupported file format.",
                Errors.Position => "Error: playback position invalid.",
                Errors.Init => "Error: the device could not be initialized.",
                Errors.Already => "The device is already initialized.",
                Errors.No3D => "Error: 3D is not supported.",
                Errors.Device => "Error: the device is not supported/valid.",
                Errors.NoInternet => "You are not connected to the internet.",
                Errors.FileFormat => "Error: unsupported file format.",
                Errors.Speaker => "Error: speakers are unavailable.",
                Errors.Codec => "Error: unsupported codec.",
                Errors.Ended => "Error: the file or channel has ended.",
                Errors.Busy => "Error: the device is busy with another process.",
                Errors.Unstreamable => "Error: the file can't be streamed using the buffered file system.",
                Errors.WmaLicense => "Error: the file is protected and can't be streamed.",
                Errors.WmaAccesDenied => "Error: the file is protected and can't be streamed.",
                Errors.Wasapi => "Error: WASAPI is not available.",
                Errors.Memory => "Error: there has been a memory error.",
                Errors.BufferLost => "Error: the sample buffer was lost.",
                Errors.Handle => "Error: the file was not loaded correctly.",
                Errors.Parameter => "Error: a wrong parameter was used.",
                Errors.NoChannel => "Error: there is no free channel.",
                Errors.Start => "Error: the audio engine has not been initialized successfully.",
                Errors.NoEAX => "Error: you do not have EAX (Environmental Audio Extensions) support.",
                Errors.SampleRate => "Error: the sample rate of this file is not valid.",
                Errors.NotFile => "Error: the file is not a valid audio file.",
                Errors.Empty => "Error: the file was not loaded correctly or is empty.",
                Errors.NoFX => "Error: the effects are not available.",
                Errors.Playing => "Error: the channel is playing.",
                Errors.NotAvailable => "Error: the requested data is not available.",
                Errors.DirectX => "Error: your directx version is not supported.",
                Errors.Timeout => "Error: connection timeout, check your internet connection and try again.",
                Errors.Mp4NoStream => "Error: your mp4 file has media segment in the wrong order (\"madt\" before \"moov\")",
                Errors.Unknown => "Error: Unknown Audio Error.",
                _ => "Error: Unknown Audio Error."
            };

            errorMessage?.CreateErrorMessage().Publish();
        }

        private string HandleNotPlayingError()
        {
            if(Init(AudioOutput.GetDefaultDevice().Index, Frequency))
            {
                if(RetryLoadingAndPLayingFile()) // success
                    return null;
                return "Error: the file is not playing despite the device being successfully been initialized, the error might come from the file being corrupted.";
            }

            return "Error: the file is not playing, the audio device might not be initialized correctly.";
        }

        private void DeviceTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            AudioDeviceModel device = AudioOutput.GetDefaultDevice();
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

        public bool ChangeOutputDevice(AudioDeviceModel device, bool fromUser = true)
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
                if ((!isPlaying && Device.DeviceType != AudioDeviceTypeEnum.HeadPhones) || Device.DeviceType == AudioDeviceTypeEnum.Speakers)
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

            Stream = Bass.CreateStream(file);
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
            Device = AudioOutput.GetDefaultDevice();
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
