using MusicPlayModels;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.MVVM.ViewModels;
using MusicPlayUI.MVVM.ViewModels.ModalViewModels;
using System;

namespace MusicPlayUI.Core.Services
{
    public class ModalService : ObservableObject, IModalService
    {
        private readonly Func<Type, ViewModel> _viewFactory;
        public ModalService(Func<Type, ViewModel> viewFactory)
        {
            _viewFactory = viewFactory;
        }

        private bool _isModalOpen;
        public bool IsModalOpen
        {
            get { return _isModalOpen; }
            private set
            {
                SetField(ref _isModalOpen, value);
            }
        }

        private ViewModel _modal;
        public ViewModel Modal
        {
            get { return _modal; }
            private set
            {
                _modal?.Dispose();
                SetField(ref _modal, value);
            }
        }

        private BaseModel _modalParameter;
        public BaseModel ModalParameter
        {
            get { return _modalParameter; }
            set { SetField(ref _modalParameter, value); }
        }

        private Action<bool> ValidationCallBack { get; set; }

        private bool _isCanceled;
        public bool IsCanceled
        {
            get => _isCanceled;
            set { SetField(ref _isCanceled, value); }
        }

        public void OpenModal(ViewNameEnum viewName, Action<bool> validationCallBack, BaseModel parameter = null)
        {
            if (!IsModalOpen)
            {
                IsModalOpen = true;
                ValidationCallBack = validationCallBack;
                ModalParameter = parameter;
                SetModal(viewName);
            }
        }

        private void SetModal(ViewNameEnum viewName)
        {
            switch (viewName)
            {
                case ViewNameEnum.CreatePlaylist:
                    Modal = _viewFactory.Invoke(typeof(CreatePlaylistViewModel));
                    break;
                case ViewNameEnum.ConfirmAction:
                    Modal = _viewFactory.Invoke(typeof(ValidationModalViewModel));
                    break;
                case ViewNameEnum.UpdateShortcut:
                    Modal = _viewFactory.Invoke(typeof(UpdateShortcutViewModel));
                    break;
            }
        }

        public void CloseModal(bool cancel = false)
        {
            IsCanceled = cancel;
            IsModalOpen = false;

            ValidationCallBack.Invoke(IsCanceled);
        }
    }
}
