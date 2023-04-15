using System.Windows.Controls;
using System.Windows.Input;

namespace MusicPlayUI.MVVM.Views
{
    /// <summary>
    /// Logique d'interaction pour ArtistLibraryView.xaml
    /// </summary>
    public partial class ArtistLibraryView : UserControl
    {
        public ArtistLibraryView()
        {
            InitializeComponent();
        }

        private void RootGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (SearchTextBox.IsFocused) // unfocus it by setting focus to the main window
            {
                FocusManager.SetFocusedElement(App.Current.MainWindow, App.Current.MainWindow);
            }
        }
    }
}
