using AudioHandler.Models;

namespace AudioHandler
{
    public interface IAudioPlayback
    {
        /// <summary>
        /// Manage the EQ settings
        /// </summary>
        EQManager EQManager { get; }

        /// <summary>
        /// The device currently used
        /// </summary>
        DeviceModel Device { get; }
        /// <summary>
        /// The frequency of the playback (44100 kHz by default)
        /// </summary>
        int Frequency { get; }
        /// <summary>
        /// wether the current stream is looping or not
        /// </summary>
        bool IsLooping { get; }
        /// <summary>
        /// wehter the current stream is playing or not
        /// </summary>
        bool IsPlaying { get; }
        /// <summary>
        /// The current position in the track
        /// </summary>
        TimeSpan Position { get; set; }
        /// <summary>
        /// The current stream
        /// </summary>
        int Stream { get; }
        /// <summary>
        /// The current volume
        /// </summary>
        int Volume { get; set; }
        /// <summary>
        /// The step by how much the volume increase or decrease by default
        /// </summary>
        int VolumeStep { get; set; }
        /// <summary>
        /// wether the output device changes automatically when a new device is connected or a device get disconnected
        /// </summary>
        bool AutoChangeOutputDevice { get; }

        event Action StreamChanged;
        event Action<int> VolumeChanged;
        event Action<bool> IsPlayingChanged;
        event Action IsLoopingChanged;
        event Action DeviceChanged;

        /// <summary>
        /// change the value of <paramref name="AutoChangeOutputDevice"/>
        /// </summary>
        /// <param name="value"></param>
        void AutoChangeOutput(bool value);

        /// <summary>
        /// Try to change the output device to the specified one
        /// </summary>
        /// <param name="device">the device to init</param>
        /// <param name="fromUser">the user is the one who chose this device</param>
        /// <returns></returns>
        bool ChangeOutputDevice(DeviceModel device, bool fromUser = true);
        /// <summary>
        /// decrease the volume by the <paramref name="VolumeStep"/> value
        /// </summary>
        void DecreaseVolume();
        /// <summary>
        /// decrease the volume with the specified step
        /// </summary>
        /// <param name="step"> how much the volume decreases</param>
        void DecreaseVolume(int step);
        /// <summary>
        /// stop the playback and free all resources related to it and the current output device
        /// </summary>
        void Dispose();
        /// <summary>
        /// get the current volume value
        /// </summary>
        /// <returns></returns>
        int GetCurrentVolume();
        /// <summary>
        /// increase the volume by the <paramref name="VolumeStep"/> value
        /// </summary>
        void IncreaseVolume();
        /// <summary>
        /// Mute or unmute the playback by setting the global volume to 0 or restoring it to its previous value (before being muted)
        /// </summary>
        void Mute();
        /// <summary>
        /// increase the volume witht the specified step
        /// </summary>
        /// <param name="step"></param>
        void IncreaseVolume(int step);
        /// <summary>
        /// init the current device with the current frequency and the default options
        /// </summary>
        void Init();
        /// <summary>
        /// init the specified device with the specified frequency
        /// </summary>
        /// <param name="device"></param>
        /// <param name="frequency"></param>
        /// <returns></returns>
        bool Init(int device, int frequency);
        /// <summary>
        /// Load a file and create a new stream
        /// </summary>
        /// <param name="file"></param>
        void Load(string file);
        /// <summary>
        /// Load and start the playback of the file
        /// </summary>
        /// <param name="file"></param>
        void LoadAnPlay(string file);
        /// <summary>
        /// loop or unloop the current stream
        /// </summary>
        void Loop();
        void Loop(bool loop);
        /// <summary>
        /// pause the playback
        /// </summary>
        void Pause();
        /// <summary>
        /// start or resume the playback of the loaded file (current stream)
        /// </summary>
        void Play();

        /// <summary>
        /// Stop the playback by freeing the buffer and all resources related to the stream
        /// </summary>
        void Stop();

    }
}