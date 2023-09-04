using MusicPlayModels;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicPlayUI.MVVM.ViewModels.ModalViewModels
{
    public class ValidationModalViewModel : ModalViewModel
    {
        private ConfirmActionModel _validationMessage;
        public ConfirmActionModel ValidationMessage
        {
            get => _validationMessage;
            set { SetField(ref _validationMessage, value); }
        }

        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }
        public ValidationModalViewModel(IModalService modalService, INavigationService navigationService) : base(modalService, navigationService)
        {
            ConfirmCommand = new RelayCommand(() => _modalService.CloseModal());
            CancelCommand = new RelayCommand(() => _modalService.CloseModal(true));

            Load();
        }

        private void Load()
        {
            ValidationMessage = (ConfirmActionModel)_modalService.ModalParameter;
        }
    }
}
