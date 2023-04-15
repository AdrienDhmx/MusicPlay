using MusicPlayModels;
using MusicPlayUI.Core.Enums;

namespace MusicPlayUI.Core.Services.Interfaces
{
    public interface IWindowService
    {
        void CloseWindow(ViewNameEnum viewName);
        void OpenWindow(ViewNameEnum viewName, BaseModel parameter = null);
    }
}