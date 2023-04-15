using MusicPlayModels;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.MVVM.ViewModels;
using System;

namespace MusicPlayUI.Core.Services.Interfaces
{
    public interface IModalService
    {
        bool IsCanceled { get; set; }
        bool IsModalOpen { get; }

        ViewModel Modal { get; }
        BaseModel ModalParameter { get; }

        void CloseModal(bool cancel = false);
        void OpenModal(ViewNameEnum viewName, Action<bool> ValidationCallBack, BaseModel parameter = null);
    }
}