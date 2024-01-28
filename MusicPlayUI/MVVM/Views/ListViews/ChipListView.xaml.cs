using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MusicPlay.Database.Models;

namespace MusicPlayUI.MVVM.Views.ListViews
{
    /// <summary>
    /// Interaction logic for ChipListView.xaml
    /// </summary>
    public partial class ChipListView : UserControl
    {
        public ChipListView()
        {
            InitializeComponent();
        }

        public ObservableCollection<Tag> Genres
        {
            get { return (ObservableCollection<Tag>)GetValue(GenresProperty); }
            set { SetValue(GenresProperty, value); }
        }

        public static readonly DependencyProperty GenresProperty =
            DependencyProperty.Register("Genres", typeof(ObservableCollection<Tag>), typeof(ChipListView), new PropertyMetadata(new ObservableCollection<Tag>()));

        public ICommand NavigateToGenreCommand
        {
            get { return (ICommand)GetValue(NavigateToGenreCommandProperty); }
            set { SetValue(NavigateToGenreCommandProperty, value); }
        }

        public static readonly DependencyProperty NavigateToGenreCommandProperty =
            DependencyProperty.Register("NavigateToGenreCommand", typeof(ICommand), typeof(ChipListView), new PropertyMetadata(null));
    }
}
