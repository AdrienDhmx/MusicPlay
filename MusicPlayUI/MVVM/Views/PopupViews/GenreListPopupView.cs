﻿using System;
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
using MusicPlayModels.MusicModels;

namespace MusicPlayUI.MVVM.Views.PopupViews
{
    /// <summary>
    /// Interaction logic for GenreListPopupView.xaml
    /// </summary>
    public partial class GenreListPopupView : UserControl
    {
        public GenreListPopupView()
        {
            InitializeComponent();
        }

        public ObservableCollection<GenreModel> Genres
        {
            get { return (ObservableCollection<GenreModel>)GetValue(GenresProperty); }
            set { SetValue(GenresProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Genre.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty GenresProperty =
            DependencyProperty.Register("Genre", typeof(ObservableCollection<GenreModel>), typeof(GenreListPopupView), new PropertyMetadata(new ObservableCollection<GenreModel>()));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(GenreListPopupView), new PropertyMetadata(null));
    }
}