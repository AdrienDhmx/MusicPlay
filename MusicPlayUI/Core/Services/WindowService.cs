using MusicPlayModels;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.ViewModels;
using MusicPlayUI.MVVM.ViewModels.SettingsViewModels;
using MusicPlayUI.MVVM.Views.Windows;
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

        public WindowService(Func<Type, Window> viewFactory)
        {
            _viewFactory = viewFactory;
        }

        private List<WindowModel> _windows = new();

        public void OpenWindow(ViewNameEnum viewName, BaseModel parameter = null)
        {
            // window already open
            if (_windows.Any(w => w.ViewName == viewName))
            {
                WindowModel window = _windows.Find(w => w.ViewName == viewName);

                // update the data (for track window properties the track changes)
                window.Window.Update(parameter);

                // find window and bring it to view (in the forefront)
                foreach (Window w in App.Current.Windows)
                {
                    if (w.Name == viewName.ViewEnumToString())
                    {
                        w.WindowState = WindowState.Normal;
                        w.Focus();
                    }
                }
            }
            else
            {
                WindowModel window = new()
                {
                    ViewName = viewName,
                    Window = OpenWindow(viewName)
                };

                if (window.Window != null)
                {
                    _windows.Add(window);
                }
            }
        }

        private ViewModel OpenWindow(ViewNameEnum viewName)
        {
            Window w;
            switch (viewName)
            {
                case ViewNameEnum.Visualizer:
                    w = _viewFactory.Invoke(typeof(VisualizerParametersWindow));
                    w.Show();
                    return (ViewModel)w.DataContext;
                default:
                    return null;
            }
        }

        public void CloseWindow(ViewNameEnum viewName)
        {
            if (_windows.Any(w => w.ViewName == viewName))
            {
                foreach (Window window in App.Current.Windows)
                {
                    if (window.Name.ToViewNameEnum() == viewName)
                    {
                        window.Close();
                        _windows.Remove(_windows.Find(w => w.ViewName == viewName));
                    }
                }
            }
        }
    }

    internal class WindowModel
    {
        public ViewNameEnum ViewName { get; set; }

        public ViewModel Window { get; set; }
    }
}
