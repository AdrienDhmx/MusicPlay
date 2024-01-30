using DynamicScrollViewer;
using MusicPlay.Database.Models;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Models;
using MusicPlayUI.MVVM.ViewModels.AppBars;
using System;
using System.Windows;
using System.Windows.Input;

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

        public static AppBar AppBar => App.State.AppBar;

        private Thickness _topMargin = AppBar is null ? new(0, 60, 0, 0) : new(0, AppBar.Height, 0, 0);
        public Thickness TopMargin
        {
            get
            {
                return _topMargin;
            }
            internal set
            {
                SetField(ref _topMargin, value);
            }
        }

        public ICommand OnScrollCommand { get; }
        public ViewModel()
        {
            OnScrollCommand = new RelayCommand<OnScrollEvent>(OnScrollEvent);
        }

        public virtual void Dispose()
        {

        }

        public virtual void OnScrollEvent(OnScrollEvent e)
        {
            // save the scroll offset
            State.ScrollOffset = e.VerticalOffset;
        }

        /// <summary>
        /// A method used to initialize / update the App bar style <br></br>
        /// Note: this method is called by <see cref="AppBars.AppBar"/> itself when the theme changes to update its owm style
        /// </summary>
        public virtual void UpdateAppBarStyle() { }

        /// <summary>
        /// A method used to initiliazed the view model data. <br></br>
        /// Note: this method calls <see cref="UpdateAppBarStyle"/>
        /// </summary>
        public virtual void Init()
        {
            UpdateAppBarStyle();
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
