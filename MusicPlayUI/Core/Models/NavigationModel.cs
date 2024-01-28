using System;
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
        public BaseModel Parameter
        {
            get => _parameter;
            set => SetField(ref _parameter, value);
        }

        private NavigationModel _childViewModel;
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
    }
}
