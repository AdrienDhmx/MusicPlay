using MusicPlayModels;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.MVVM.ViewModels;
using System;

namespace MusicPlayUI.Core.Services.Interfaces
{
    public interface IModalService
    {
        /// <summary>
        /// Was the <see cref="Modal"/> canceled ?
        /// </summary>
        bool IsCanceled { get; set; }

        /// <summary>
        /// Is the <see cref="Modal"/> open ?
        /// </summary>
        bool IsModalOpen { get; }

        /// <summary>
        /// The current modal or last modal to have been opened
        /// </summary>
        ViewModel Modal { get; }

        /// <summary>
        /// The parameter of the <see cref="Modal"/>
        /// </summary>
        BaseModel ModalParameter { get; }

        /// <summary>
        /// Close the current <see cref="Modal"/>
        /// </summary>
        /// <param name="cancel">Wether the action the modal is for has been canceled by the user</param>
        void CloseModal(bool cancel = false);

        /// <summary>
        /// Open a modal if <see cref="IsModalOpen"/> is false and update the <see cref="Modal"/> property
        /// </summary>
        /// <param name="viewName">The name of the view of the modal to open</param>
        /// <param name="ValidationCallBack">The callback to execute when the modal is closed, if the parameter is true then action has been canceled</param>
        /// <param name="parameter">An optional parameter to pass to the modal</param>
        void OpenModal(ViewNameEnum viewName, Action<bool> ValidationCallBack, BaseModel parameter = null);
    }
}