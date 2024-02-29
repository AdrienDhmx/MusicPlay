using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicScrollViewer;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models.DataBaseModels;
using MusicPlayUI.Core.Services.Interfaces;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class BaseQueueViewModel : ViewModel
    {
        const int _trackPerPage = 25;
        internal IQueueService _queueService;
        public IQueueService QueueService
        {
            get => _queueService;
            set => SetField(ref _queueService, value);
        }

        private ObservableCollection<QueueTrack> _queueTracks = new();
        public ObservableCollection<QueueTrack> QueueTracks
        {
            get => _queueTracks;
            set => SetField(ref _queueTracks, value);
        }

        public BaseQueueViewModel(IQueueService queueService)
        {
            QueueService = queueService;

            QueueService.QueueChanged += QueueService_QueueChanged;
            QueueService.PreviewPlayingTrackChanged += QueueService_PlayingTrackChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            QueueService.QueueChanged -= QueueService_QueueChanged;
            QueueService.PreviewPlayingTrackChanged -= QueueService_PlayingTrackChanged;
        }

        public override void OnScrollEvent(OnScrollEvent e)
        {
            if(e.IsScrollingForward)
            {
                if (e.VerticalOffset < e.Sender.ScrollableHeight - 200)
                {
                    return;
                }

                int lastTrackIndex = _queueService.GetTrackIndex(_queueTracks.Last().Track) + 1;
                int newEndIndex = lastTrackIndex + _trackPerPage - 1;
                if(newEndIndex > _queueService.Queue.Tracks.Count - 1)
                {
                    newEndIndex = _queueTracks.Count - 1;
                }
                AddTracks(lastTrackIndex, newEndIndex);
            }

            base.OnScrollEvent(e);
        }

        internal void AddTracks(int start, int end)
        {
            for(int i = start; i <= end; i++)
            {
                _queueTracks.Add(_queueService.Queue.Tracks[i]);
            }
        }

        private void QueueService_PlayingTrackChanged(int index)
        {
            if(!_queueTracks.Any(qt => qt.Track.Id == _queueService.Queue.Tracks[index].Track.Id))
            {
                int lastTrackIndex = _queueService.GetTrackIndex(_queueTracks.Last().Track) + 1;
                int newEndIndex = index + _trackPerPage / 2;
                if (newEndIndex > _queueService.Queue.Tracks.Count - 1)
                {
                    newEndIndex = _queueTracks.Count - 1;
                }
                AddTracks(lastTrackIndex, newEndIndex);
            }
        }

        private void QueueService_QueueChanged()
        {
            if (_queueService.Queue.PlayingTrack.IsNotNull())
            {
                int playingTrackIndex = _queueService.GetPlayingTrackIndex();
                _queueTracks = new();
                AddTracks(0, playingTrackIndex + _trackPerPage / 2);
            }
        }

        public override void Init()
        {
            QueueService_QueueChanged();

            base.Init();
        }


    }
}
