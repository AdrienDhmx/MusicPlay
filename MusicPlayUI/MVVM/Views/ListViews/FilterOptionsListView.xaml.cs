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
using MusicPlayUI.MVVM.Models;

namespace MusicPlayUI.MVVM.Views.ListViews
{
    /// <summary>
    /// Interaction logic for FilterOptionsListView.xaml
    /// </summary>
    public partial class FilterOptionsListView : UserControl
    {
        public FilterOptionsListView()
        {
            InitializeComponent();
        }

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(FilterOptionsListView), new PropertyMetadata(string.Empty));

        public ObservableCollection<FilterModel> Filters
        {
            get { return (ObservableCollection<FilterModel>)GetValue(FiltersProperty); }
            set { SetValue(FiltersProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Filters.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FiltersProperty =
            DependencyProperty.Register("Filters", typeof(ObservableCollection<FilterModel>), typeof(FilterOptionsListView), new PropertyMetadata(new ObservableCollection<FilterModel>(), OnFiltersChanged));

        public ICommand AddFilterCommand
        {
            get { return (ICommand)GetValue(AddFilterCommandProperty); }
            set { SetValue(AddFilterCommandProperty, value); }
        }

        public static readonly DependencyProperty AddFilterCommandProperty =
            DependencyProperty.Register("AddFilterCommand", typeof(ICommand), typeof(FilterOptionsListView), new PropertyMetadata(null));

        private static void OnFiltersChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FilterOptionsListView filterOptionsListView = (FilterOptionsListView)d;

            if(e.NewValue is ObservableCollection<FilterModel> filters)
            {
                filterOptionsListView.Filters = filters;
            }
        }
    }
}
