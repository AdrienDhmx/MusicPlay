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
        bool IsQueueDrawerOpen { get; }
        bool IsPopupOpen { get; }

        event Action IsFullScreenChanged;

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

        /// <summary>
        /// Go into or escapce full screen
        /// </summary>
        void ToggleFullScreen();
        /// <summary>
        /// Open or close the queue drawer
        /// </summary>
        void ToggleQueueDrawer();

        /// <summary>
        /// Navigate to the view specified by <paramref name="viewName"/>
        /// </summary>
        /// <param name="viewName">The view to navigate to</param>
        /// <param name="parameter">The parameter to pass to the view</param>
        void NavigateTo(string viewName, BaseModel parameter = null);

        /// <summary>
        /// Navigate to the view specified by <paramref name="viewName"/>
        /// </summary>
        /// <param name="viewName">The view to navigate to</param>
        /// <param name="parameter">The parameter to pass to the view</param>
        /// <param name="saveView">Wether to save the view to go back to it if necessary <see cref="NavigateBack"/> </param>
        void NavigateTo(ViewNameEnum viewName, BaseModel parameter = null, bool saveView = true);

        /// <summary>
        /// Navigate to the previous view
        /// </summary>
        void NavigateBack();

        /// <summary>
        /// Open the popup and set a new PopupViewModel
        /// </summary>
        /// <param name="viewName">The popup to open</param>
        /// <param name="parameter">The parameter to pass to the popup</param>
        void OpenPopup(ViewNameEnum viewName, BaseModel parameter);

        /// <summary>
        /// Close the popup
        /// </summary>
        void ClosePopup();

        /// <summary>
        /// Only Close the popup if the parameter is the same as the current popup
        /// </summary>
        /// <param name="parameter">The parameter to compare to the current popup parameter</param>
        void ClosePopup(BaseModel parameter);
    }
}