﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using MusicPlay.Database.Models;
using MusicPlayUI.Core.Enums;
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
                if(_state is not null)
                {
                    _state.PropertyChanged -= State_PropertyChanged;
                }

                SetField(ref _state, value); 

                if(_state is not null)
                {
                    _state.PropertyChanged += State_PropertyChanged;
                }
            }
        }

        public NavigationModel(ViewModel viewModel, NavigationState state)
        {
            ViewModel = viewModel;
            _state = state;
            if(state != null)
                _state.PropertyChanged += State_PropertyChanged;
        }

        private void State_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateState();
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

        public LibraryNavigationState(BaseModel parameters) : base(parameters)
        {
        }

        public LibraryNavigationState(NavigationState navigationState) : base(navigationState.Parameter, navigationState.ChildViewModel)
        {
            ScrollOffset = navigationState.ScrollOffset;
        }
    }
}
