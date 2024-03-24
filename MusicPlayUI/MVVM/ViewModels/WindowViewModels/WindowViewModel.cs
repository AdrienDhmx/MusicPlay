using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services.Interfaces;

namespace MusicPlayUI.MVVM.ViewModels.WindowViewModels
{
    public abstract class WindowViewModel : ViewModel
    {
        protected readonly IWindowService _windowService;
        protected readonly ViewNameEnum _viewName;

        public ICommand CloseWindowCommand { get; }
        public ICommand MinimizeWindowCommand { get; }
        public ICommand MaximizeWindowCommand { get; }
        public WindowViewModel(IWindowService windowService, ViewNameEnum viewName)
        {
            _windowService = windowService;
            _viewName = viewName;
            CloseWindowCommand = new RelayCommand(CloseWindow);
            MaximizeWindowCommand = new RelayCommand(MaximizeWindow);
            MinimizeWindowCommand = new RelayCommand(MinimizeWindow);
        }


        protected void MinimizeWindow()
        {
            _windowService.MinimizeWindow(_viewName);
        }

        protected void MaximizeWindow()
        {
            _windowService.MaximizeWindow(_viewName);
        }

        protected void CloseWindow()
        {
            _windowService.CloseWindow(_viewName);
        }
    }
}
