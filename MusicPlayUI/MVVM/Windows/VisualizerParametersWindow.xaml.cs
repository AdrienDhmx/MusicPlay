using Microsoft.Extensions.DependencyInjection;
using MusicPlayUI.MVVM.ViewModels.SettingsViewModels;
using MusicPlayUI.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MusicPlayUI.MVVM.Views.Windows
{
    /// <summary>
    /// Interaction logic for VisualizerParametersWindow.xaml
    /// </summary>
    public partial class VisualizerParametersWindow : Window
    {
        public VisualizerParametersWindow()
        {
            InitializeComponent();
            DataContext = RegisterServices.service.GetService<VisualizerSettingViewModel>();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void SettingScroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            SettingScroll.ScrollToVerticalOffset(SettingScroll.VerticalOffset - e.Delta / 3);
        }

        private void MinimizeBtn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
    }
}
