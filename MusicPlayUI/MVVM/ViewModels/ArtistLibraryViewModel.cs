using AudioHandler;
using DataBaseConnection.DataAccess;
using MusicPlay.Language;
using MusicPlayModels;
using MusicPlayModels.MusicModels;
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
    public class ArtistLibraryViewModel : ViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IQueueService _queueService;

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

        private ObservableCollection<ArtistModel> _bindedArtists;
        public ObservableCollection<ArtistModel> BindedArtists
        {
            get
            {
                return _bindedArtists;
            }
            set
            {
                _bindedArtists = value;
                OnPropertyChanged(nameof(BindedArtists));
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
        public ArtistLibraryViewModel(INavigationService navigationService, IQueueService queueService)
        {
            _navigationService = navigationService;
            _queueService = queueService;

            _sortingPopupTimer = new(50);
            _sortingPopupTimer.Elapsed += SortingPopupTimer_Elapsed;
            _sortingPopupTimer.AutoReset = false;

            PlayArtistCommand = new RelayCommand<ArtistModel>(async (artist) =>
            {
                if (artist is not null)
                {
                    _queueService.SetNewQueue(await ArtistServices.GetArtistTracks(artist.Id), new(artist.Name, ModelTypeEnum.Artist, artist.Id), artist.Cover, null, false);
                }
            });

            NavigateToArtistCommand = new RelayCommand<ArtistModel>((artist) =>
            {
                _navigationService.NavigateTo(ViewNameEnum.SpecificArtist, artist);
            });

            OpenArtistPopupCommand = new RelayCommand<ArtistModel>((artist) =>
            {
                if (artist is not null)
                {
                    _navigationService.OpenPopup(ViewNameEnum.ArtistPopup, artist);
                }
            });

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

            Task.Run(InitData);
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

        private async void FilterSearch()
        {
            BindedArtists = new(await SearchHelper.FilterArtists(SelectedFilters.ToList(), SearchText, SelectedSorting.SortType, SelectedSorting.IsAscending));

            NoArtistFoundVisbility = BindedArtists.Count == 0;
            ArtistCount = $"{BindedArtists.Count} of {TotalArtistCount}";

        }

        private async void InitData()
        {
            (TotalArtistCount, int _, int _) = await DataAccess.Connection.GetNumberOfEntries();

            List<FilterModel> artistsFilter = FilterFactory.GetArtistTypeFilter();

            List<FilterModel> temp = SearchHelper.GetSelectedFilters(false).ToList();

            foreach (FilterModel filter in temp)
            {
                for (int i = 0; i < artistsFilter.Count; i++)
                {
                    if (filter.Equals(artistsFilter[i]))
                    {
                        filter.Name = artistsFilter[i].Name;
                        artistsFilter.RemoveAt(i);
                    }
                }
            }

            ArtistTypeFilters = new(artistsFilter);
            SelectedFilters = new(temp);

            SortBy = SortFactory.GetSortMenu<ArtistModel>();
            SelectedSorting = SortBy.ToList().Find(s => s.IsSelected);

            FilterSearch();
        }

        public override void Update(BaseModel parameter = null)
        {
            FilterSearch();
        }
    }
}
