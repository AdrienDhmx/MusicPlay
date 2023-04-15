using DataBaseConnection.DataAccess;
using MusicFilesProcessor;
using MusicFilesProcessor.Helpers;
using MusicPlayModels.MusicModels;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

namespace MusicPlayUI.MVVM.Views
{
    /// <summary>
    /// Logique d'interaction pour PlaylistLibraryView.xaml
    /// </summary>
    public partial class PlaylistLibraryView : UserControl
    {
        public PlaylistLibraryView()
        {
            InitializeComponent();
        }

        private void RootGrid_Drop(object sender, DragEventArgs e)
        {
            DropIndicatorBorder.Visibility = Visibility.Hidden;
        }

        private void RootGrid_DragEnter(object sender, DragEventArgs e)
        {
            DragDropEffects effects = DragDropEffects.None;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var path = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                if (Directory.Exists(path))
                {
                    effects = DragDropEffects.Copy;
                    DropIndicatorBorder.Visibility = Visibility.Visible;
                }
            }

            e.Effects = effects;
        }

        private void RootGrid_DragLeave(object sender, DragEventArgs e)
        {
            DropIndicatorBorder.Visibility = Visibility.Hidden;
        }
    }
}
