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
using MusicPlay.Database.Helpers;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class AlbumLibraryViewModel : LibraryViewModel
    {
        private readonly IQueueService _queueService;
        private readonly ICommandsManager _commandsManager;
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

        private string _albumsHeader;
        public string AlbumCount
        {
            get { return _albumsHeader; }
            set
            {
                AppBar.Subtitle = value;
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

        public ICommand PlayAlbumCommand { get; }
        public ICommand NavigateToAlbumCommand { get; }
        public ICommand OpenAlbumPopupCommand { get; }
        public AlbumLibraryViewModel(IQueueService queueService, ICommandsManager commandsManager)
        {
            _queueService = queueService;
            _commandsManager = commandsManager;

            PlayAlbumCommand = _commandsManager.PlayNewQueueCommand;
            NavigateToAlbumCommand = _commandsManager.NavigateToAlbumCommand;
            OpenAlbumPopupCommand = _commandsManager.OpenAlbumPopupCommand;
        }

        public override void Dispose()
        {
            SearchHelper.SaveFilter(SettingsEnum.AlbumFilter, AppliedFilters);
            SearchHelper.SaveOrder(SettingsEnum.AlbumOrder, SortBy);
            base.Dispose();
        }

        internal override void Sort()
        {
            if(SortBy is not null && !IsLoading)
            {
                AllFilteredAlbums = AllFilteredAlbums.SortAlbums(SortBy);
                PaginateData();
            }
        }

        internal override void FilterSearch()
        {
            AllFilteredAlbums = SearchHelper.FilterAlbum(AppliedFilters, SearchText, SortBy);
            LibraryState.Page = 1; // reset pagination
            PaginateData();

            NoAlbumFoundVisibility = Albums.Count == 0;
            TotalFilteredItems = AllFilteredAlbums.Count;
            AlbumCount = $"{TotalFilteredItems} of {TotalItemCount}";
        }

        private void PaginateData()
        {
            int endIndex = LibraryState.Page * LibraryState.ItemPerPage;

            List<Album> temp = [];
            if(endIndex > AllFilteredAlbums.Count)
            {
                temp = AllFilteredAlbums;
            }
            else
            {
                temp = [..AllFilteredAlbums.Take(endIndex)]; 
            }

            if(!temp.AreEquals([..Albums?? new()]))
            {
                Albums = new(temp);
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

        public override void InitFilters()
        {
            AppliedFilters ??= SearchHelper.GetSelectedFilters();

            Filters.Filters = new(FilterFactory.GetGenreFilter());
            Filters.AddFilters(FilterFactory.GetPrimaryArtistFilter());
            Filters.AddFilters(FilterFactory.GetAlbumTypeFilter());

            base.InitFilters();
        }

        public override void Init()
        {
            UpdateAppBarStyle();
            AppBar.Title = "My Albums";
            SortOptions = SortFactory.GetSortMenu<Album>();
            base.Init();
            IsLoading = false;
            TotalItemCount = Album.Count();
            FilterSearch();
        }

        public override void Update(BaseModel parameter = null)
        {
            FilterSearch();
        }       
    }
}
