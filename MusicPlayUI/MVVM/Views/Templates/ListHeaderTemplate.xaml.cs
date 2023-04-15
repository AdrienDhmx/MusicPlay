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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicPlayUI.MVVM.Views.Templates
{
    /// <summary>
    /// Interaction logic for ListHeaderTemplate.xaml
    /// </summary>
    public partial class ListHeaderTemplate : UserControl
    {
        public ListHeaderTemplate()
        {
            InitializeComponent();
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public ICommand HeaderCommand
        {
            get { return (ICommand)GetValue(HeaderCommandProperty); }
            set { SetValue(HeaderCommandProperty, value); }
        }

        public ICommand PlayCommand
        {
            get { return (ICommand)GetValue(PlayCommandProperty); }
            set { SetValue(PlayCommandProperty, value); }
        }

        public ICommand PlayShuffledCommand
        {
            get { return (ICommand)GetValue(PlayShuffledCommandProperty); }
            set { SetValue(PlayShuffledCommandProperty, value); }
        }

        public Visibility ContentVisibility
        {
            get { return (Visibility)GetValue(ContentVisibilityProperty); }
            set { SetValue(ContentVisibilityProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(ListHeaderTemplate), new PropertyMetadata(""));

        public static readonly DependencyProperty HeaderCommandProperty =
            DependencyProperty.Register("HeaderCommand", typeof(ICommand), typeof(ListHeaderTemplate), new PropertyMetadata(null));

        public static readonly DependencyProperty PlayCommandProperty =
            DependencyProperty.Register("PlayCommand", typeof(ICommand), typeof(ListHeaderTemplate), new PropertyMetadata(null));

        public static readonly DependencyProperty PlayShuffledCommandProperty =
            DependencyProperty.Register("PlayShuffledCommand", typeof(ICommand), typeof(ListHeaderTemplate), new PropertyMetadata(null));

        public static readonly DependencyProperty ContentVisibilityProperty =
            DependencyProperty.Register("ContentVisibility", typeof(Visibility), typeof(ListHeaderTemplate), new PropertyMetadata(Visibility.Visible));
    }
}
