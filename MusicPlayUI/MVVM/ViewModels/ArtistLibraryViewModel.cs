using AudioHandler;
using DynamicScrollViewer;
using MusicPlay.Database.Enums;
using MusicPlay.Database.Models;
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
using System.Windows.Input;

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

        private string _artistHeader;
        public string ArtistCount
        {
            get { return _artistHeader; }
            set
            {
                SetField(ref _artistHeader, value);
                AppBar.Subtitle = value;
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

        private int TotalArtistCount { get; set; }

        public ICommand PlayArtistCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public ICommand OpenArtistPopupCommand { get; }
        public ArtistLibraryViewModel(IQueueService queueService, ICommandsManager commandsManager)
        {
            _queueService = queueService;
            _commandsManager = commandsManager;

            PlayArtistCommand = _commandsManager.PlayNewQueueCommand;
            NavigateToArtistCommand = _commandsManager.NavigateToArtistCommand;
            OpenArtistPopupCommand = _commandsManager.OpenArtistPopupCommand;
        }

        public override void Dispose()
        {
            SearchHelper.SaveFilter(SettingsEnum.ArtistFilter, AppliedFilters);
            SearchHelper.SaveOrder(SettingsEnum.ArtistOrder, SortBy);
            base.Dispose();
        }

        internal override void Sort()
        {
            if(SortBy is not null && !IsLoading)
            {
                AllFilteredArtists = AllFilteredArtists.SortArtists(SortBy);
                PaginateData();
            }
        }

        internal override void FilterSearch()
        {
            AllFilteredArtists = SearchHelper.FilterArtists(AppliedFilters, SearchText, SortBy);
            LibraryState.Page = 1; // reset pagination
            PaginateData();

            TotalFilteredItems = AllFilteredArtists.Count;
            NoArtistFoundVisbility = Artists.Count == 0;
            ArtistCount = $"{TotalFilteredItems} of {TotalArtistCount}";
        }

        private void PaginateData()
        {
            int endIndex = LibraryState.Page * LibraryState.ItemPerPage;

            List<Artist> temp = [];
            if (endIndex > AllFilteredArtists.Count)
            {
                temp = AllFilteredArtists;
            }
            else
            {
                temp = new(AllFilteredArtists.Take(endIndex));
            }

            if (!temp.AreEquals([.. Artists ?? new()]))
            {
                Artists = new(temp);
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

        public override void InitFilters()
        {
            AppliedFilters ??= SearchHelper.GetSelectedFilters(false);

            Filters.Filters = new(FilterFactory.GetGenreFilter());
            Filters.AddFilters(FilterFactory.GetArtistRoleFilter());
            Filters.AddFilters(FilterFactory.GetAlbumTypeFilter());

            base.InitFilters();
        }

        public override void Init()
        {
            AppBar.Title = "My Artists";
            UpdateAppBarStyle();
            SortOptions = SortFactory.GetSortMenu<Artist>();
            base.Init();
            IsLoading = false;
            TotalArtistCount = Artist.Count();
            FilterSearch();
        }

        public override void Update(BaseModel parameter = null)
        {
            FilterSearch();
        }
    }
}
