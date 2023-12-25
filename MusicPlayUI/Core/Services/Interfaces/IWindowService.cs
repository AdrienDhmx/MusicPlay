using MusicPlayModels;
using MusicPlayUI.Core.Enums;

namespace MusicPlayUI.Core.Services.Interfaces
{
    public interface IWindowService
    {

        /// <summary>
        /// Open a new window if there isn't a window corresponding to this ViewNameEnum
        /// Otherwise update the current window with the parameter and bring it to the front
        /// </summary>
        /// <param name="viewName">The window to open</param>
        /// <param name="parameter">The parameter to pass to the window view model</param>
        void OpenWindow(ViewNameEnum viewName, BaseModel parameter = null);

        /// <summary>
        /// Minimize the window corresponding to this ViewNameEnum
        /// </summary>
        /// <param name="viewName">The Window to close</param>
        void MinimizeWindow(ViewNameEnum viewName);

        /// <summary>
        /// Maximize or normalize the window corresponding to this ViewNameEnum
        /// </summary>
        /// <param name="viewName">The Window to close</param>
        void MaximizeWindow(ViewNameEnum viewName);

        /// <summary>
        /// Closes the window corresponding to this ViewNameEnum
        /// </summary>
        /// <param name="viewName">The Window to close</param>
        void CloseWindow(ViewNameEnum viewName);
    }
}