using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using MusicPlay.Database.Models;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.MVVM.ViewModels;

namespace MusicPlayUI.Core.Models
{
    public class NavigationModel : ObservableObject
    {
        public ViewModel ViewModel { get; private set; }

        private NavigationState _state;
        public NavigationState State
        {
            get => _state;
            set 
            {
                SetField(ref _state, value); 
            }
        }

        public NavigationModel(ViewModel viewModel, NavigationState state)
        {
            ViewModel = viewModel;
            State = state;
        }

        public void UpdateState()
        {
            OnPropertyChanged(nameof(State));
            ViewModel.OnStateChanged();
        }
    }

    public class NavigationState : ObservableObject
    {
        private BaseModel _parameter;
        private double _scrollOffset;
        private NavigationModel _childViewModel;

        public BaseModel Parameter
        {
            get => _parameter;
            set => SetField(ref _parameter, value);
        }

        public double ScrollOffset
        {
            get => _scrollOffset;
            set => SetField(ref _scrollOffset, value);
        }

        public NavigationModel ChildViewModel
        {
            get => _childViewModel;
            set
            {
                SetField(ref _childViewModel, value);
            }
        }

        public NavigationState(BaseModel parameters)
        {
            Parameter = parameters;
        }

        public NavigationState(BaseModel parameters, NavigationModel childViewModel)
        {
            Parameter = parameters;
            ChildViewModel = childViewModel;
        }
    }

    public class LibraryNavigationState : NavigationState
    {
        private int _page = 1;
        private int _itemPerPage = 25;

        private string _searchText = string.Empty;
        private SortModel _sortBy;
        private LibraryFilters _appliedFilters;

        public int Page
        {
            get => _page;
            set => SetField(ref _page, value);
        }

        public int ItemPerPage
        {
            get => _itemPerPage;
            set => SetField(ref _itemPerPage, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => SetField(ref _searchText, value);
        }

        public LibraryFilters AppliedFilters
        {
            get => _appliedFilters;
            set => SetField(ref _appliedFilters, value);
        }

        public SortModel SortBy
        {
            get { return _sortBy; }
            set
            {
                SetField(ref _sortBy, value);
            }
        }

        public LibraryNavigationState(BaseModel parameters) : base(parameters)
        {
        }

        public LibraryNavigationState(NavigationState navigationState) : base(navigationState.Parameter, navigationState.ChildViewModel)
        {
            ScrollOffset = navigationState.ScrollOffset;
        }
    }
}
