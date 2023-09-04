using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DataBaseConnection.DataAccess;
using MusicPlayModels;
using MusicPlayModels.MusicModels;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;
using static Humanizer.On;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class GenreLibraryViewModel : ViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IQueueService _queueService;
        private readonly IModalService _modalService;
        private readonly ICommandsManager _commandsManager;
        private ObservableCollection<UITagModel> _bindedGenres = new();
        public ObservableCollection<UITagModel> BindedGenres
        {
            get { return _bindedGenres; }
            set
            {
                _bindedGenres = value;
                OnPropertyChanged(nameof(BindedGenres));
            }
        }

        private List<UITagModel> _allTags;
        public List<UITagModel> AllTags
        {
            get => _allTags;
            set
            {
                SetField(ref _allTags, value);
            }
        }

        private string _genreCount;
        public string GenreCount
        {
            get { return _genreCount; }
            set
            {
                SetField(ref _genreCount, value);
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                SetField(ref _searchText, value);
                Task.Run(Search);
            }
        }

        public ICommand CreateGenreCommand { get; }
        public ICommand PlayGenreCommand { get; }
        public ICommand NavigateToGenreCommand { get; }
        public ICommand OpenTagPopupCommand { get; }
        public GenreLibraryViewModel(INavigationService navigationService, IQueueService queueService, IModalService modalService, ICommandsManager commandsManager)
        {
            _navigationService = navigationService;
            _queueService = queueService;
            _modalService = modalService;
            _commandsManager = commandsManager;

            CreateGenreCommand = new RelayCommand(() =>
            {
                _modalService.OpenModal(Core.Enums.ViewNameEnum.CreateTag, (canceled) =>
                {
                    if (!canceled)
                    {
                        Update();
                    }
                });
            });

            NavigateToGenreCommand = new RelayCommand<UITagModel>((genre) =>
            {
                navigationService.NavigateTo(Core.Enums.ViewNameEnum.SpecificGenre, genre);
            });

            PlayGenreCommand = new RelayCommand<UITagModel>((genre) => Task.Run(async () =>
            {
                List<TrackModel> tracks = await DataAccess.Connection.GetTracksFromAlbums(genre.Albums.Select(a => a.Id));
                tracks = await DataAccess.Connection.GetTracksFromArtists(genre.Artists.Select(a => a.Id));
                tracks = tracks.DistinctBy(t => t.Id).ToList();

                queueService.SetNewQueue(tracks, new(genre.Name, Core.Enums.ModelTypeEnum.Genre, genre.Id), genre.Cover);
            }));

            OpenTagPopupCommand = _commandsManager.OpenTagPopupCommand;

            Update();
        }

        private void Search()
        {
            BindedGenres = new(AllTags.Where(t => t.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)));
            GenreCount = $"{BindedGenres.Count} of {AllTags.Count}";
        }

        public override async void Update(BaseModel parameter = null)
        {
            AllTags = await (await DataAccess.Connection.GetAllTags()).ToUIGenreModel();
            
            BindedGenres = new(AllTags);
            GenreCount = $"{BindedGenres.Count} of {AllTags.Count}";
        }
    }
}
