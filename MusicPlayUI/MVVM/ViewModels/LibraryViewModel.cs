using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicScrollViewer;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Models;
using MusicPlayUI.Core.Services;
using MusicPlayUI.MVVM.Models;

namespace MusicPlayUI.MVVM.ViewModels
{
    public abstract class LibraryViewModel : ViewModel
    {
        public LibraryNavigationState LibraryState
        {
            get
            {
                if (base.State is LibraryNavigationState libraryNavigationState)
                    return libraryNavigationState;
                LibraryNavigationState navigationState = new LibraryNavigationState(base.State);
                base.State = navigationState;
                return navigationState;
            }
            set => base.State = value;
        }

        private int _totalFilteredItems = 0;
        private int _totalItemCount = 0;

        private string _searchText = string.Empty;
        private SortModel _sortBy;
        private ObservableCollection<SortModel> _sortOptions;

        private bool _isLoading = true;

        public int TotalFilteredItems
        {
            get => _totalFilteredItems;
            set => SetField(ref _totalFilteredItems, value);
        }

        public int TotalItemCount
        {
            get => _totalItemCount;
            set => SetField(ref _totalItemCount, value);
        }

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (!IsLoading)
                {
                    if (string.IsNullOrWhiteSpace(SearchText) && string.IsNullOrWhiteSpace(value))
                        return; // empty and no changes

                    SetField(ref _searchText, value);
                    LibraryState.SearchText = value;
                    Task.Run(FilterSearch);
                }
            }
        }

        public SortModel SortBy
        {
            get { return _sortBy; }
            set
            {
                SetField(ref _sortBy, value);
                if (value is not null)
                {
                    LibraryState.SortBy = value;
                }
            }
        }

        public ObservableCollection<SortModel> SortOptions
        {
            get => _sortOptions;
            set
            {
                SetField(ref _sortOptions, value);
            }
        }

        private bool _isSortOptionsPopupOpen = false;
        public bool IsSortOptionsPopupOpen
        {
            get { return _isSortOptionsPopupOpen; }
            set
            {
                SetField(ref _isSortOptionsPopupOpen, value);
            }
        }

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public ICommand OpenSortingPopupCommand { get; }
        public ICommand SortCommand { get; }
        public LibraryViewModel()
        {

            OpenSortingPopupCommand = new RelayCommand(() =>
            {
                IsSortOptionsPopupOpen = !IsSortOptionsPopupOpen;
            });

            SortCommand = new RelayCommand<SortModel>(UpdateSortBy);
        }

        internal virtual (bool, int, int) CanLoadNewItems(OnScrollEvent onScrollEvent)
        {
            if (onScrollEvent == null)
                return (false, 0, 0);

            if (!onScrollEvent.IsScrollingVertically || !onScrollEvent.IsScrollingForward)
                return (false, 0, 0);

            double offsetThreshold = onScrollEvent.Sender.ScrollableHeight - 200;
            if (onScrollEvent.VerticalOffset > offsetThreshold)
            {
                int startIndex = LibraryState.Page * LibraryState.ItemPerPage;
                LibraryState.Page++;
                int endIndex = LibraryState.Page * LibraryState.ItemPerPage;

                if (endIndex > TotalFilteredItems)
                {
                    endIndex = TotalFilteredItems;
                }

                if (startIndex >= endIndex) // all the data is already loaded
                {
                    return (false, 0, 0);
                }
                return (true, startIndex, endIndex);
            }
            return (false, 0, 0);
        }

        public override void Dispose()
        {

        }

        internal virtual void UpdateSortBy(SortModel sortBy)
        {
            if (sortBy is null)
                return;

            if (SortBy.Type == sortBy.Type)
            {
                SortBy.IsAscending = !SortBy.IsAscending;
                sortBy.IsAscending = SortBy.IsAscending;
            }
            else
            {
                SortBy = sortBy;
                foreach (SortModel sortOption in SortOptions)
                {
                    if (sortBy.Type == sortOption.Type)
                    {
                        sortOption.IsSelected = true;
                        continue;
                    }
                    sortOption.IsSelected = false;
                }
            }
            Sort();
        }

        internal virtual void Sort()
        {

        }

        internal virtual void FilterSearch()
        {

        }

        public override void Init()
        {
            base.Init();

            _searchText = LibraryState.SearchText;
            SortBy = LibraryState.SortBy;

            if (SortBy is null)
                return;

            foreach (SortModel sortOption in SortOptions)
            {
                if (SortBy.Type == sortOption.Type)
                {
                    sortOption.IsSelected = true;
                    continue;
                }
                sortOption.IsSelected = false;
            }
        }

        public override void UpdateAppBarStyle()
        {
            AppBar.SetStyle(AppTheme.Palette.PrimaryContainer, 0, 0, false);
        }
    }
}
