using AudioHandler;
using DynamicScrollViewer;
using MusicPlay.Database.Enums;
using MusicPlay.Database.Models;
using MusicPlay.Language;

using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Helpers;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class ArtistLibraryViewModel : LibraryViewModel
    {
        private readonly IQueueService _queueService;
        private readonly ICommandsManager _commandsManager;
        private bool _noArtistFoundVisibility = false;
        public bool NoArtistFoundVisbility
        {
            get { return _noArtistFoundVisibility; }
            set
            {
                _noArtistFoundVisibility = value;
                OnPropertyChanged(nameof(NoArtistFoundVisbility));
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                if (string.IsNullOrWhiteSpace(SearchText) && string.IsNullOrWhiteSpace(value))
                    return;

                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                Task.Run(FilterSearch);
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

        private string _artistHeader;
        public string ArtistCount
        {
            get { return _artistHeader; }
            set
            {
                SetField(ref _artistHeader, value);
            }
        }

        private List<Artist> AllFilteredArtists = [];

        private ObservableCollection<Artist> _artists;
        public ObservableCollection<Artist> Artists
        {
            get
            {
                return _artists;
            }
            set
            {
                _artists = value;
                OnPropertyChanged(nameof(Artists));
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

        private ObservableCollection<FilterModel> _artistTypeFilters;
        public ObservableCollection<FilterModel> ArtistTypeFilters
        {
            get => _artistTypeFilters;
            set
            {
                SetField(ref _artistTypeFilters, value);
            }
        }

        private ObservableCollection<FilterModel> _tagFilters;
        public ObservableCollection<FilterModel> TagFilters
        {
            get => _tagFilters;
            set
            {
                SetField(ref _tagFilters, value);
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

        private int TotalArtistCount { get; set; }

        public ICommand PlayArtistCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public ICommand OpenFiltersCommand { get; }
        public ICommand OpenArtistPopupCommand { get; }
        public ICommand AddFilterCommand { get; }
        public ICommand RemoveFilterCommand { get; }
        public ICommand OpenSortingPopupCommand { get; }
        public ICommand SortCommand { get; }
        public ArtistLibraryViewModel(IQueueService queueService, ICommandsManager commandsManager)
        {
            _queueService = queueService;
            _commandsManager = commandsManager;

            _sortingPopupTimer = new(50);
            _sortingPopupTimer.Elapsed += SortingPopupTimer_Elapsed;
            _sortingPopupTimer.AutoReset = false;

            PlayArtistCommand = new RelayCommand<Artist>(async (artist) =>
            {
                if (artist is not null)
                {
                    _queueService.SetNewQueue(await ArtistServices.GetArtistTracks(artist.Id), artist, artist.Name, artist.Cover);
                }
            });

            NavigateToArtistCommand = _commandsManager.NavigateToArtistCommand;
            OpenArtistPopupCommand = _commandsManager.OpenArtistPopupCommand;

            OpenFiltersCommand = new RelayCommand(() =>
            {
                IsFilterOpen = !IsFilterOpen;
            });

            OpenSortingPopupCommand = new RelayCommand(() =>
            {
                if (!IsSortingOpen)
                {
                    _isSortingOpen = true;
                    OnPropertyChanged(nameof(IsSortingOpen));
                }
            });

            AddFilterCommand = new RelayCommand<FilterModel>((filter) =>
            {
                if (filter is not null)
                {
                    SelectedFilters.Add(filter);
                    if (filter.FilterType == FilterEnum.ArtistType)
                    {
                        ArtistTypeFilters.Remove(filter);
                    } 
                    else if(filter.FilterType == FilterEnum.Genre)
                    {
                        TagFilters.Remove(filter);
                    }
                    FilterSearch();
                }
            });

            RemoveFilterCommand = new RelayCommand<FilterModel>((filter) =>
            {
                SelectedFilters.Remove(filter);
                if (!string.IsNullOrWhiteSpace(filter.Name))
                {
                    if (filter.FilterType == FilterEnum.ArtistType)
                    {
                        ArtistTypeFilters.Add(filter);
                        ArtistTypeFilters = new(ArtistTypeFilters.OrderBy(f => f.Name));
                    } 
                    else if(filter.FilterType == FilterEnum.Genre)
                    {
                        TagFilters.Add(filter);
                        TagFilters = new(TagFilters.OrderBy(f => f.Name));
                    }
                }
                FilterSearch();
            });

            SortCommand = new RelayCommand<SortModel>((sort) =>
            {
                if (SelectedSorting.SortType == sort.SortType)
                {
                    SelectedSorting.IsAscending = !SelectedSorting.IsAscending;
                    foreach (SortModel s in SortBy)
                    {
                        if (SelectedSorting.SortType == s.SortType)
                        {
                            s.IsAscending = SelectedSorting.IsAscending;
                        }
                    }
                }
                else
                {
                    foreach (SortModel s in SortBy)
                    {
                        if (sort.SortType == s.SortType)
                        {
                            s.IsSelected = true;
                        }
                        else
                        {
                            s.IsSelected = false;

                        }
                    }
                    SelectedSorting = sort;
                }
                FilterSearch();
            });
        }

        public override void Dispose()
        {
            _sortingPopupTimer.Elapsed -= SortingPopupTimer_Elapsed;
        }

        private void SortingPopupTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _isSortingOpen = false;
            OnPropertyChanged(nameof(IsSortingOpen));
        }

        private void FilterSearch()
        {
            AllFilteredArtists = SearchHelper.FilterArtists([.. SelectedFilters], SearchText, SelectedSorting.SortType, SelectedSorting.IsAscending);
            LibraryState.Page = 1; // reset pagination
            PaginateData();

            TotalFilteredItems = AllFilteredArtists.Count;
            NoArtistFoundVisbility = Artists.Count == 0;
            ArtistCount = $"{TotalFilteredItems} of {TotalArtistCount}";
        }

        private void PaginateData()
        {
            int endIndex = LibraryState.Page * LibraryState.ItemPerPage;

            if (endIndex > AllFilteredArtists.Count)
            {
                Artists = new(AllFilteredArtists);
            }
            else
            {
                Artists = new(AllFilteredArtists.Take(endIndex));
            }
        }

        public override void OnScrollEvent(OnScrollEvent e)
        {
            (bool canAddItems, int startIndex, int endIndex) = base.CanLoadNewItems(e);
            if (canAddItems)
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    Artists.Add(AllFilteredArtists[i]);
                }
            }
            base.OnScrollEvent(e);
        }

        public override void Init()
        {
            base.Init();
            InitData();
        }

        private void InitData()
        {
            TotalArtistCount = Artist.Count();

            List<FilterModel> tagFilters = FilterFactory.GetGenreFilter();
            List<FilterModel> artistsFilter = FilterFactory.GetArtistTypeFilter();

            List<FilterModel> temp = [.. SearchHelper.GetSelectedFilters(false)];

            foreach (FilterModel filter in temp)
            {
                for (int i = 0; i < tagFilters.Count; i++)
                {
                    if (filter.Equals(tagFilters[i]))
                    {
                        filter.Name = tagFilters[i].Name;
                        tagFilters.RemoveAt(i);
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
            }

            TagFilters = new(tagFilters.OrderBy(f => f.Name));
            ArtistTypeFilters = new(artistsFilter.OrderBy(f => f.Name));
            SelectedFilters = new(temp);

            SortBy = SortFactory.GetSortMenu<Artist>();
            SelectedSorting = SortBy.ToList().Find(s => s.IsSelected);

            FilterSearch();
        }

        public override void Update(BaseModel parameter = null)
        {
            FilterSearch();
        }
    }
}
