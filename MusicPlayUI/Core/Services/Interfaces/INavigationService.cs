using MusicPlayModels;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.MVVM.ViewModels;
using System;

namespace MusicPlayUI.Core.Services.Interfaces
{
    public interface INavigationService
    {
        bool IsFullScreen { get; }

        event Action IsFullScreenChanged;

        bool IsPopupOpen { get; }

        ViewModel CurrentViewModel { get; }
        ViewNameEnum CurrentViewName { get; }
        BaseModel CurrentViewParameter { get; }

        event Action<ViewNameEnum> CurrentViewChanged;

        ViewModel SecondaryViewModel { get; }
        ViewNameEnum ScdViewName { get; }
        BaseModel ScdViewParameter { get; }

        ViewModel PopupViewModel { get; }
        ViewNameEnum PopupViewName { get; }
        BaseModel PopupViewParameter { get; }

        void SwitchFullScreen();

        void NavigateTo(string viewName, BaseModel parameter = null);
        void NavigateTo(ViewNameEnum viewName, BaseModel parameter = null, bool saveView = true);
        void NavigateBack();

        /// <summary>
        /// Open the popup and set a new PopupViewModel
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="parameter"></param>
        void OpenPopup(ViewNameEnum viewName, BaseModel parameter);

        /// <summary>
        /// Close the popup
        /// </summary>
        void ClosePopup();

        /// <summary>
        /// Only Close the popup if the parameter is the same as the current popup one
        /// </summary>
        /// <param name="parameter"></param>
        void ClosePopup(BaseModel parameter);
    }
}