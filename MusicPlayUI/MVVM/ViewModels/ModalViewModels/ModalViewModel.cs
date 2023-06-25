using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services.Interfaces;

namespace MusicPlayUI.MVVM.ViewModels.ModalViewModels
{
    public class ModalViewModel : ViewModel
    {
        internal readonly IModalService _modalService;

        public ICommand CloseModalCommand { get; }
        public ModalViewModel(IModalService modalService)
        {
            _modalService = modalService;

            CloseModalCommand = new RelayCommand(() => CloseModal(true));

            Update(_modalService.ModalParameter);
        }

        public virtual void CloseModal(bool canceled = false)
        {
            _modalService.CloseModal(canceled);
        }
    }
}
