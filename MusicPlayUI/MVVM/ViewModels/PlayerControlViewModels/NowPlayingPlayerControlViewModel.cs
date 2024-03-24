using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using AudioHandler;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services.Interfaces;

namespace MusicPlayUI.MVVM.ViewModels.PlayerControlViewModels
{
    public class NowPlayingPlayerControlViewModel : ViewModel
    {
        private IQueueService _queueService;
        public IQueueService QueueService
        {
            get => _queueService;
            set
            {
                SetField(ref _queueService, value);
            }
        }

        private IAudioTimeService _audioService;
        public IAudioTimeService AudioService
        {
            get => _audioService;
            set { SetField(ref _audioService, value); }
        }

        private IAudioPlayback _audioPlayback;
        public IAudioPlayback AudioPlayback
        {
            get { return _audioPlayback; }
            set { SetField(ref _audioPlayback, value); }
        }

        public ICommand ShuffleCommand { get; }
        public ICommand RepeatCommand { get; }
        public ICommand PlayPauseCommand { get; }
        public ICommand NextTrackCommand { get; }
        public ICommand PreviousTrackCommand { get; }
        public NowPlayingPlayerControlViewModel(IAudioTimeService audioTimeService, IAudioPlayback audioPlayback, IQueueService queueService)
        {
            _queueService = queueService;
            AudioService = audioTimeService;
            AudioPlayback = audioPlayback;

            PlayPauseCommand = new RelayCommand(_audioService.PlayPause);

            PreviousTrackCommand = new RelayCommand(() =>
            {
                _queueService.PreviousTrack();
            });

            NextTrackCommand = new RelayCommand(() =>
            {
                _queueService.NextTrack();
            });

            ShuffleCommand = new RelayCommand(() => Task.Run(_queueService.Shuffle));

            RepeatCommand = new RelayCommand(() =>
            {
                if (_audioPlayback.IsLooping)
                {
                    _audioService.Loop(); // Remove the loop
                }
                else
                {
                    if (_queueService.Queue.IsOnRepeat)
                    {
                        _audioService.Loop(); // set the loop
                    }
                    _queueService.Repeat(); // Remove or set the repeat
                }
            });
            _queueService = queueService;
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}
