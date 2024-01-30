using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.Core.Helpers;
using System.Configuration;
using System.Timers;
using MusicPlay.Database.Models;
using MusicPlayUI.Core.Models;
using MusicPlayUI.MVVM.ViewModels.PopupViewModels;
using DynamicScrollViewer;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class AlbumLibraryViewModel : LibraryViewModel
    {
        private readonly IQueueService _queueService;

        private string _searchText = "";
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (!IsLoading)
                {
                    if (string.IsNullOrWhiteSpace(SearchText) && string.IsNullOrWhiteSpace(value))
                        return; // empty and no changes

                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    Task.Run(FilterSearch);
                }
            }
        }

        private bool _isLoading = true;
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        private bool _noAlbumFoundVisibility = false;
        public bool NoAlbumFoundVisibility
        {
            get { return _noAlbumFoundVisibility; }
            set
            {
                _noAlbumFoundVisibility = value;
                OnPropertyChanged(nameof(NoAlbumFoundVisibility));
            }
        }

        private bool _isFilterOpen = false;
        public bool IsFilterOpen
        {
            get { return _isFilterOpen; }
            set
            {
                _isFilterOpen = value;
                OnPropertyChanged(nameof(IsFilterOpen));
            }
        }

        private bool _isSortingOpen = false;
        public bool IsSortingOpen
        {
            get { return _isSortingOpen; }
            set
            {
                _sortingPopupTimer.Start();
            }
        }

        private readonly Timer _sortingPopupTimer;

        private string _albumsHeader;
        public string AlbumCount
        {
            get { return _albumsHeader; }
            set
            {
                SetField(ref _albumsHeader, value);
            }
        }

        private List<Album> AllFilteredAlbums { get; set; } = [];

        private ObservableCollection<Album> _albums;
        public ObservableCollection<Album> Albums
        {
            get
            {
                return _albums;
            }
            set
            {
                _albums = value;
                OnPropertyChanged(nameof(Albums));
            }
        }

        private ObservableCollection<FilterModel> _selectedFilters;
        public ObservableCollection<FilterModel> SelectedFilters
        {
            get { return _selectedFilters; }
            set
            {
                SetField(ref _selectedFilters, value);
            }
        }

        private ObservableCollection<FilterModel> _genreFilters;
        public ObservableCollection<FilterModel> GenreFilters
        {
            get => _genreFilters;
            set
            {
                SetField(ref _genreFilters, value);
            }
        }

        private ObservableCollection<FilterModel> _artistsFilters;
        public ObservableCollection<FilterModel> ArtistsFilters
        {
            get => _artistsFilters;
            set
            {
                SetField(ref _artistsFilters, value);
            }
        }

        private ObservableCollection<FilterModel> _albumTypeFilters;
        public ObservableCollection<FilterModel> AlbumTypeFilters
        {
            get => _albumTypeFilters;
            set
            {
                SetField(ref _albumTypeFilters, value);
            }
        }

        private SortModel _selectedSorting;
        public SortModel SelectedSorting
        {
            get { return _selectedSorting; }
            set
            {
                SetField(ref _selectedSorting, value);
            }
        }

        private ObservableCollection<SortModel> _sortBy;
        public ObservableCollection<SortModel> SortBy
        {
            get => _sortBy;
            set
            {
                SetField(ref _sortBy, value);
            }
        }

        public ICommand PlayAlbumCommand { get; }
        public ICommand NavigateToAlbumCommand { get; }
        public ICommand OpenCloseFiltersCommand { get; }
        public ICommand AddFilterCommand { get; }
        public ICommand RemoveFilterCommand { get; }
        public ICommand OpenSortingPopupCommand { get; }
        public ICommand SortCommand { get; }
        public ICommand OpenAlbumPopupCommand { get; }
        public AlbumLibraryViewModel(IQueueService queueService)
        {
            _queueService = queueService;

            _sortingPopupTimer = new(50);
            _sortingPopupTimer.Elapsed += SortingPopupTimer_Elapsed;
            _sortingPopupTimer.AutoReset = false;

            PlayAlbumCommand = new RelayCommand<Album>((album) =>
            {
                if (album is not null)
                {
                    _queueService.SetNewQueue(album.Tracks, album, album.Name, album.AlbumCover, orderTracks: true);
                }
            });

            NavigateToAlbumCommand = new RelayCommand<Album>((album) =>
            {
                App.State.NavigateTo<AlbumViewModel>(album);
            });

            OpenAlbumPopupCommand = new RelayCommand<Album>((album) =>
            {
                if (album is not null)
                {
                    App.State.OpenPopup<AlbumPopupViewModel>(album);
                }
            });

            OpenCloseFiltersCommand = new RelayCommand(() =>
            {
                IsFilterOpen = !IsFilterOpen;
            });

            AddFilterCommand = new RelayCommand<FilterModel>((filter) =>
            {
                SelectedFilters.Add(filter);
                if(filter.FilterType == FilterEnum.Artist)
                {
                    ArtistsFilters.Remove(filter);
                }
                else if(filter.FilterType == FilterEnum.Genre)
                {
                    GenreFilters.Remove(filter);
                }
                else if (filter.FilterType == FilterEnum.AlbumType)
                {
                    AlbumTypeFilters.Remove(filter);
                }

                FilterSearch();
            });

            RemoveFilterCommand = new RelayCommand<FilterModel>((filter) =>
            {
                SelectedFilters.Remove(filter);
                if (!string.IsNullOrWhiteSpace(filter.Name))
                {
                    if (filter.FilterType == FilterEnum.Artist)
                    {
                        ArtistsFilters.Add(filter);
                        ArtistsFilters = new(ArtistsFilters.OrderBy(f => f.Name));
                    }
                    else if (filter.FilterType == FilterEnum.Genre)
                    {
                        GenreFilters.Add(filter);
                        GenreFilters = new(GenreFilters.OrderBy(f => f.Name));
                    }
                    else if (filter.FilterType == FilterEnum.AlbumType)
                    {
                        AlbumTypeFilters.Add(filter);
                        AlbumTypeFilters = new(AlbumTypeFilters.OrderByDescending(f => f.Name));
                    }
                }
                FilterSearch();
            });

            OpenSortingPopupCommand = new RelayCommand(() =>
            {
                if(!IsSortingOpen)
                {
                    _isSortingOpen = true;
                    OnPropertyChanged(nameof(IsSortingOpen));
                }
            });

            SortCommand = new RelayCommand<SortModel>((sortModel) =>
            {
                if(sortModel is not null)
                {
                    if(SelectedSorting.SortType == sortModel.SortType)
                    {
                        SelectedSorting.IsAscending = !SelectedSorting.IsAscending;
                        foreach (SortModel sort in SortBy)
                        {
                            if(SelectedSorting.SortType == sort.SortType)
                            {
                                sort.IsAscending = SelectedSorting.IsAscending;
                            }
                        }
                    }
                    else
                    {
                        foreach (SortModel sort in SortBy)
                        {
                            if (sortModel.SortType == sort.SortType)
                            {
                                sort.IsSelected = true;
                            }
                            else
                            {
                                sort.IsSelected = false;
                            }
                        }
                        SelectedSorting = sortModel;
                    }
                    FilterSearch();
                }
            });
        }

        public override void Dispose()
        {
            _sortingPopupTimer.Elapsed -= SortingPopupTimer_Elapsed;
            GC.SuppressFinalize(this);
        }

        private void SortingPopupTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _isSortingOpen = false;
            OnPropertyChanged(nameof(IsSortingOpen));
        }

        private void FilterSearch()
        {
            AllFilteredAlbums = SearchHelper.FilterAlbum([.. SelectedFilters], SearchText, SelectedSorting.SortType, SelectedSorting.IsAscending);
            LibraryState.Page = 1; // reset pagination
            PaginateData();

            NoAlbumFoundVisibility = Albums.Count == 0;
            TotalFilteredItems = AllFilteredAlbums.Count;
            AlbumCount = $"{TotalFilteredItems} of {TotalItemCount}";
        }

        private void PaginateData()
        {
            int endIndex = LibraryState.Page * LibraryState.ItemPerPage;

            if(endIndex > AllFilteredAlbums.Count)
            {
                Albums = new(AllFilteredAlbums);
            }
            else
            {
                Albums = new(AllFilteredAlbums.Take(endIndex)); 
            }
        }

        public override void OnScrollEvent(OnScrollEvent e)
        {
            (bool canAddItems, int startIndex, int endIndex) = base.CanLoadNewItems(e);
            if(canAddItems)
            {
                for(int i = startIndex; i < endIndex; i++)
                {
                    Albums.Add(AllFilteredAlbums[i]);
                }
            }
            base.OnScrollEvent(e);
        }

        public override void Init()
        {
            base.Init();

            InitData();
            TotalItemCount = Album.Count();
            FilterSearch();
        }

        public override void Update(BaseModel parameter = null)
        {
            FilterSearch();
        }

        private void InitData()
        {
            List <FilterModel> genresFilter = FilterFactory.GetGenreFilter();
            List<FilterModel> artistsFilter = FilterFactory.GetAlbumArtistFilter();
            List<FilterModel> albumFilter = FilterFactory.GetAlbumTypeFilter();

            List<FilterModel> temp = [.. SearchHelper.GetSelectedFilters()];

            foreach (FilterModel filter in temp)
            {
                for (int i = 0; i < genresFilter.Count; i++)
                {
                    if (filter.Equals(genresFilter[i]))
                    {
                        filter.Name = genresFilter[i].Name;
                        genresFilter.RemoveAt(i);
                    }
                }
                for (int i = 0; i < artistsFilter.Count; i++)
                {
                    if (filter.Equals(artistsFilter[i]))
                    {
                        filter.Name = artistsFilter[i].Name;
                        artistsFilter.RemoveAt(i);
                    }
                }
                for (int i = 0; i < albumFilter.Count; i++)
                {
                    if (filter.Equals(albumFilter[i]))
                    {
                        filter.Name = albumFilter[i].Name;
                        albumFilter.RemoveAt(i);
                    }
                }
            }

            SelectedFilters = new(temp);
            GenreFilters = new(genresFilter);
            ArtistsFilters = new(artistsFilter);
            AlbumTypeFilters = new(albumFilter);

            SortBy = SortFactory.GetSortMenu<Album>();
            SelectedSorting = SortBy.ToList().Find(s => s.IsSelected);
        }
       
    }
}
