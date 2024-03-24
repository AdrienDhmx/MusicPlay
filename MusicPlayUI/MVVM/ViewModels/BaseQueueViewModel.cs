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
        internal bool _saveScrollOffset { get; set;} = true;

        private IAudioTimeService _audioTimeService;
        public IAudioTimeService AudioTimeService
        {
            get => _audioTimeService;
            set => SetField(ref  _audioTimeService, value);
        }

        internal IQueueService _queueService;
        public IQueueService QueueService
        {
            get => _queueService;
            set => SetField(ref _queueService, value);
        }

        public BaseQueueViewModel(IQueueService queueService, IAudioTimeService audioTimeService)
        {
            QueueService = queueService;
            _audioTimeService = audioTimeService;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override void OnScrollEvent(OnScrollEvent e)
        {
            _scrollViewer = e.Sender;
            if (_saveScrollOffset)
            {
                base.OnScrollEvent(e);
            }
        }

        public override void Init()
        {
            base.Init();
        }


    }
}
