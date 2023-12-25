using MusicPlayModels;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Helpers;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.ViewModels;
using MusicPlayUI.MVVM.ViewModels.SettingsViewModels;
using MusicPlayUI.MVVM.Views.Windows;
using MusicPlayUI.MVVM.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MusicPlayUI.Core.Services
{
    public class WindowService : IWindowService
    {
        private readonly Func<Type, Window> _viewFactory;
        private readonly List<WindowModel> _windows = new();

        public WindowService(Func<Type, Window> viewFactory)
        {
            _viewFactory = viewFactory;
        }

        public void OpenWindow(ViewNameEnum viewName, BaseModel parameter = null)
        {
            // window already open
            WindowModel window = _windows.Find(w => w.ViewName == viewName);
            if (window.IsNotNull())
            {
                // update the data (for track window properties the track changes)
                window.ViewModel.Update(parameter);
                window.Window.WindowState = WindowState.Normal; // if it was minimize open it
                window.Window.Focus(); // bring the window to the front
            }
            else
            {
                window = OpenWindow(viewName);
                if (window.IsNotNull())
                {
                    window.ViewModel.Update(parameter);
                    _windows.Add(window);
                }
            }
        }

        private WindowModel OpenWindow(ViewNameEnum viewName)
        {
            WindowModel window = new()
            {
                ViewName = viewName,
            };
            switch (viewName)
            {
                case ViewNameEnum.Visualizer:
                    window.Window = _viewFactory.Invoke(typeof(VisualizerParametersWindow));
                    break;
                case ViewNameEnum.ArtistProperties:
                    window.Window = _viewFactory.Invoke(typeof(EditArtistWindow));
                    break;
                default:
                    return null;
            }
            window.Window.Show();
            window.ViewModel = (ViewModel)window.Window.DataContext;
            return window;
        }

        public void MinimizeWindow(ViewNameEnum viewName)
        {
            WindowModel window = _windows.Find(w => w.ViewName == viewName);
            if (window.IsNotNull())
            {
                window.Window.WindowState = WindowState.Minimized;
            }
        }

        public void MaximizeWindow(ViewNameEnum viewName)
        {
            WindowModel window = _windows.Find(w => w.ViewName == viewName);
            if (window.IsNotNull())
            {
                if(window.Window.WindowState == WindowState.Maximized)
                {
                    window.Window.WindowState = WindowState.Normal;
                } 
                else
                {
                    window.Window.WindowState = WindowState.Maximized;
                }
            }
        }

        public void CloseWindow(ViewNameEnum viewName)
        {
            WindowModel window = _windows.Find(w => w.ViewName == viewName);
            if (window.IsNotNull())
            {
                window.Window.Close();
                window.ViewModel.Dispose();
                _windows.Remove(window);
            }
        }
    }

    internal class WindowModel
    {
        public ViewNameEnum ViewName { get; set; }

        public Window Window { get; set; }

        public ViewModel ViewModel { get; set; }
    }
}
