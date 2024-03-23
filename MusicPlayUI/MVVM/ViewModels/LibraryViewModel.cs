using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicScrollViewer;
using MusicPlay.Database.Helpers;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Helpers;
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
                AppState.CurrentView.State = navigationState;
                return navigationState;
            }
        }

        private int _totalFilteredItems = 0;
        private int _totalItemCount = 0;

        private string _searchText = string.Empty;
        private LibraryFilters _appliedFilters = null;
        private LibraryFilters _filters = new();
        private SortModel _sortBy;
        private ObservableCollection<SortModel> _sortOptions;

        private bool _isFilterMenuOpen = false;
        private bool _isSortOptionsPopupOpen = false;
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

        public LibraryFilters AppliedFilters
        {
            get => _appliedFilters;
            set => SetField(ref _appliedFilters, value);
        }

        public LibraryFilters Filters
        {
            get => _filters;
            set => SetField(ref _filters, value);
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

        public bool IsFilterMenuOpen
        {
            get => _isFilterMenuOpen;
            set => SetField(ref _isFilterMenuOpen, value);
        }

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

        public ICommand AddFilterCommand { get; }
        public ICommand RemoveFilterCommand { get; }
        public ICommand InverseFilterCommand { get; }
        public ICommand ClearFiltersCommand { get; }
        public ICommand SortCommand { get; }
        public ICommand OpenCloseFilterMenuCommand { get; }
        public ICommand OpenSortingPopupCommand { get; }
        public LibraryViewModel()
        {
            AddFilterCommand = new RelayCommand<FilterModel>(AddFilter);
            RemoveFilterCommand = new RelayCommand<FilterModel>(RemoveFilter);
            InverseFilterCommand = new RelayCommand<FilterModel>((filter) =>
            {
                filter.IsNegative = !filter.IsNegative;
                FilterSearch();
            });
            ClearFiltersCommand = new RelayCommand(() =>
            {
                AppliedFilters.Filters.Clear();

                Filters.AddFilters([.. AppliedFilters.TagFilters]);
                AppliedFilters.TagFilters.Clear();

                Filters.AddFilters([.. AppliedFilters.ArtistRoleFilters]);
                AppliedFilters.ArtistRoleFilters.Clear();

                Filters.AddFilters([.. AppliedFilters.PrimaryArtistFilters]);
                AppliedFilters.PrimaryArtistFilters.Clear();

                Filters.AddFilters([.. AppliedFilters.AlbumTypeFilters]);
                AppliedFilters.AlbumTypeFilters.Clear();

                FilterSearch();
            });

            SortCommand = new RelayCommand<SortModel>(UpdateSortBy);

            OpenCloseFilterMenuCommand = new RelayCommand(() =>
            {
                IsFilterMenuOpen = !IsFilterMenuOpen;
            });

            OpenSortingPopupCommand = new RelayCommand(() =>
            {
                IsSortOptionsPopupOpen = !IsSortOptionsPopupOpen;
            });
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
            GC.SuppressFinalize(this);

            LibraryState.AppliedFilters = AppliedFilters;
            LibraryState.SearchText = SearchText;
            LibraryState.SortBy = SortBy;
        }

        internal virtual void AddFilter(FilterModel filter)
        {
            if (filter == null)
                return;

            AppliedFilters.AddFilter(filter);
            Filters.RemoveFilter(filter);

            FilterSearch();
        }

        internal virtual void RemoveFilter(FilterModel filter)
        {
            if (filter == null)
                return;

            AppliedFilters.RemoveFilter(filter);
            Filters.AddFilter(filter);

            FilterSearch();
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

        public override void OnScrollEvent(OnScrollEvent e)
        {
            if (e.Sender.AnimatedHeader != null)
            {
                double animationProgress = 1 - (e.Sender.AnimatedHeader.Height / e.Sender.AnimatedHeader.MaxHeight);
                AppBar.ContentOpacity = animationProgress;
            }
            base.OnScrollEvent(e);
        }

        public virtual void InitFilters()
        {
            if (AppliedFilters is not null)
            {
                for (int i = Filters.Filters.Count - 1; i >= 0; --i)
                {
                    foreach (FilterModel appliedFilter in AppliedFilters.Filters)
                    {
                        if (Filters.Filters[i].Id == appliedFilter.Id)
                        {
                            Filters.RemoveFilter(Filters.Filters[i]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method initiliaze the following properties with <see cref="LibraryState"/>:
        /// <br>- <see cref="SearchText"/></br>
        /// <br>- <see cref="AppliedFilters"/></br>
        /// <br>- <see cref="SortBy"/></br>
        /// <br>
        /// It also calls <see cref="InitFilters"/> right after initiliazing <see cref="AppliedFilters"/> (it can be null).
        /// </br>
        /// <br>
        /// It also loop through <see cref="SortOptions"/> to set the selected SortModel it it's not null (<see cref="SortBy"/>) 
        /// </br>
        /// <br>
        /// Note: <see cref="ViewModel.Init"/> is not called, nor <see cref="UpdateAppBarStyle"/>.
        /// </br>
        /// </summary>
        public override void Init()
        {
            _searchText = LibraryState.SearchText;
            OnPropertyChanged(nameof(SearchText));

            AppliedFilters = LibraryState.AppliedFilters;
            InitFilters();

            SortBy = LibraryState.SortBy;
            if (SortBy is not null)
            {
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
            else if(SortOptions.IsNotNullOrEmpty() && SortBy.IsNull())
            {
                SortBy = SortOptions.First(s => s.IsSelected);
            }
        }

        public override void UpdateAppBarStyle()
        {
            AppBar.SetStyle(AppTheme.Palette.PrimaryContainer, 0, 0, false);
            AppBar.SetForeground(AppTheme.Palette.OnBackground);
        }
    }
}
