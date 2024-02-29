using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using ManagedBass;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models;

namespace AudioHandler
{
    public class AudioEventManager
    {
        private readonly IAudioPlayback _audioPlayback;
        private readonly DispatcherTimer _audioTimer;

        /// <summary>
        /// Called when the current stream (playback) has reached the end of the file
        /// </summary>
        private Action _onStreamEndCallbacks;
        /// <summary>
        /// Called when the stream changes
        /// </summary>
        private Action _onStreamChangedCallbacks;
        /// <summary>
        /// Called at every tick with an interval of <see cref="TimerInterval"/>
        /// </summary>
        private Action _onTickCallbacks;

        /// <summary>
        /// Called once per stream when the stream is listened for more than half its total duration,
        /// Or when 4 minutes have been listened to, Whichever happens first.
        /// </summary>
        private Action _onHalfWayThroughCallbacks;

        private double _previousTickStreamPosition = -1;
        private bool _halfWayThroughPassed = false;

        public int TimerInterval { get; private set; }
        public double StreamSkippedDurationMs { get; private set; } = 0;

        public double StreamListenTimeMs => StreamPositionMs - StreamSkippedDurationMs;

        public double MaxStreamDurationMs { get; set;} = 0;
        public double StreamPositionMs { get; private set; } = 0;


        public AudioEventManager(IAudioPlayback audioPlayback, int timerInterval = 200)
        {
            _audioPlayback = audioPlayback;
            TimerInterval = timerInterval;
            _audioPlayback.StreamChanged += AudioPlayback_StreamChanged;
            _audioPlayback.IsPlayingChanged += AudioPlayback_IsPlayingChanged;

            _audioTimer = new();
            _audioTimer.Tick += AudioTimer_Tick;
            _audioTimer.Interval = TimeSpan.FromMilliseconds(TimerInterval);

            // init the properties based on the stream
            if(_audioPlayback.Stream != -1)
            {
                MaxStreamDurationMs = 1000 * Bass.ChannelBytes2Seconds(_audioPlayback.Stream, Bass.ChannelGetLength(_audioPlayback.Stream));
                StreamPositionMs = _audioPlayback.Position.TotalMilliseconds;
            }
        }

        private void AudioTimer_Tick(object sender, EventArgs e)
        {
            // update the current position
            StreamPositionMs = _audioPlayback.Position.TotalMilliseconds;

            // the stream position has been forwarded
            if (StreamPositionMs - _previousTickStreamPosition > TimerInterval)
            {
                StreamSkippedDurationMs += StreamPositionMs - _previousTickStreamPosition - TimerInterval;
            }
            // the stream position has been rewind
            else if(StreamPositionMs - _previousTickStreamPosition + TimerInterval < 0) 
            {
                StreamSkippedDurationMs += StreamPositionMs - _previousTickStreamPosition + TimerInterval;
            }

            if(!_halfWayThroughPassed && (StreamListenTimeMs >= MaxStreamDurationMs / 2 || StreamListenTimeMs > 400000)) // 4 minutes
            {
                _halfWayThroughPassed = true;
                _onHalfWayThroughCallbacks?.Invoke();
            }
            else if (StreamPositionMs >= MaxStreamDurationMs 
                || _previousTickStreamPosition == StreamPositionMs) // position has not changed and the playback is not paused => end reached
            {
                OnEndReached();
            }
            else
            {
                // update the previous stream position to current one
                _previousTickStreamPosition = StreamPositionMs;
                // trigger the registered actions
                _onTickCallbacks?.Invoke();
            }

        }

        private void OnEndReached()
        {
            _onStreamEndCallbacks?.Invoke();
        }

        private void AudioPlayback_IsPlayingChanged(bool isPlaying)
        {
            if (isPlaying)
                _audioTimer.Start();
            else
                _audioTimer.Stop();
        }

        private void AudioPlayback_StreamChanged()
        {
            _audioTimer.Stop();

            if (_audioPlayback.Stream == -1)
            {
                MaxStreamDurationMs = 1000;
                StreamPositionMs = 0;
            }
            else
            {
                MaxStreamDurationMs = 1000 * Bass.ChannelBytes2Seconds(_audioPlayback.Stream, Bass.ChannelGetLength(_audioPlayback.Stream));
                StreamPositionMs = _audioPlayback.Position.TotalMilliseconds;
                _audioTimer.Start();
            }

            _onStreamChangedCallbacks?.Invoke();

            _halfWayThroughPassed = false;
            StreamSkippedDurationMs = 0;
            _previousTickStreamPosition = -1;
        }

        /// <summary>
        /// Register an Action to execute when the stream end (the end of the playback/file has been reached).
        /// </summary>
        /// <param name="onStreamEndCallback"></param>
        public void RegisterOnStreamEndCallback(Action onStreamEndCallback)
        {
            _onStreamEndCallbacks = onStreamEndCallback;
        }

        public void RemoveOnStreamEndCallback()
        {
            _onStreamEndCallbacks = null;
        }

        /// <summary>
        /// Register an Action to be executed when the stream changes (a new file has been loaded)
        /// </summary>
        /// <param name="onStreamChangedCallback"></param>
        public void RegisterOnStreamChangedCallback(Action onStreamChangedCallback)
        {
            _onStreamChangedCallbacks = onStreamChangedCallback;
        }

        public void RemoveOnStreamChangedCallback()
        {
            _onStreamChangedCallbacks = null;
        }

        /// <summary>
        /// Register an Action to perform at every Tick (<see cref="TimerInterval"/>).<br></br>
        /// Note: The cumulated actions must be quicker than <see cref="TimerInterval"/>  - 5ms (~ time used by this class to perform its logic) 
        /// </summary>
        /// <param name="onTickCallback"></param>
        public void RegisterOnTickCallback(Action onTickCallback)
        {
            _onTickCallbacks = onTickCallback;
        }

        /// <summary>
        /// Remove a callback executed every Tick (<see cref="TimerInterval"/>).
        /// </summary>
        /// <param name="onTickCallback"></param>
        public void RemoveOnTickCallback()
        {
            _onTickCallbacks = null;
        }

        /// <summary>
        /// Register an Action to perform when half of the total duration is listened to (once per stream).<br></br>
        /// </summary>
        /// <param name="onHalfWayThroughCallback"></param>
        public void RegisterOnHalfWayThroughCallback(Action onHalfWayThroughCallback)
        {
            _onHalfWayThroughCallbacks = onHalfWayThroughCallback;
        }

        public void RemoveOnHalfWayThroughCallback(Action onHalfWayThroughCallback)
        {
            _onHalfWayThroughCallbacks = onHalfWayThroughCallback;
        }
    }
}
