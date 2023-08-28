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
using MusicPlayUI.MVVM.Models;

namespace MusicPlayUI.MVVM.Views.ListViews
{
    /// <summary>
    /// Interaction logic for GenreListView.xaml
    /// </summary>
    public partial class GenreListView : UserControl
    {
        public GenreListView()
        {
            InitializeComponent();
        }

        public ObservableCollection<UITagModel> Tags
        {
            get { return (ObservableCollection<UITagModel>)GetValue(TagsProperty); }
            set { SetValue(TagsProperty, value); }
        }

        public static readonly DependencyProperty TagsProperty =
            DependencyProperty.Register("Tags", typeof(ObservableCollection<UITagModel>), typeof(GenreListView), new PropertyMetadata(new ObservableCollection<UITagModel>()));

    }
}
