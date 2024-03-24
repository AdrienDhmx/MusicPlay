using GongSolutions.Wpf.DragDrop.Utilities;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace MusicPlayUI.MVVM.Views
{
    /// <summary>
    /// Logique d'interaction pour AlbumLibraryView.xaml
    /// </summary>
    public partial class AlbumLibraryView : UserControl
    {
        public AlbumLibraryView()
        {
            InitializeComponent();
        }

        private void RootGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SearchTextBox.IsFocused) // unfocus it by shifting the focus to the main window
            {
                FocusManager.SetFocusedElement(App.Current.MainWindow, App.Current.MainWindow);
            }
        }
    }
}
