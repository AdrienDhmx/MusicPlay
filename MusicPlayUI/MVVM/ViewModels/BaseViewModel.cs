using MusicPlay.Database.Models;
using MusicPlayUI.Core.Models;
using System;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class ViewModel : ObservableObject, IDisposable
    {
        public virtual NavigationState State
        {
            get => App.State.CurrentView?.State;
            set
            {
                if(App.State.CurrentView != null)
                {
                    App.State.CurrentView.State = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        public ViewModel()
        {
            
        }

        public virtual void Dispose()
        {

        }

        public virtual void Init()
        {

        }

        public virtual void Update(BaseModel parameter = null)
        {

        }

        public virtual void OnStateChanged()
        {
            OnPropertyChanged(nameof(State));
        }
    }
}
