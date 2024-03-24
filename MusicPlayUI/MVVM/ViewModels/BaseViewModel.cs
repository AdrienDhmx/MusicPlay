using DynamicScrollViewer;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Models;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.ViewModels.AppBars;
using System;
using System.Windows;
using System.Windows.Input;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class ViewModel : ObservableObject, IDisposable
    {
        internal bool IsActive {get; set;} = true;

        protected DynamicScrollViewer.DynamicScrollViewer _scrollViewer;

        public virtual NavigationState State
        {
            get => AppState.CurrentView?.State;
        }

        public static IAppState AppState => App.State;
        public static AppBar AppBar => AppState.AppBar;

        public bool CanScroll
        {
            get
            {
                if(_scrollViewer.IsNotNull())
                {
                    return _scrollViewer.ExtentHeight > _scrollViewer.ViewportHeight;
                }
                return false;
            }
        }

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

        /// <summary>
        /// This scroll to the top of this <see cref="_scrollViewer"/>, to scroll to the top of a child view you have to override this
        /// </summary>
        public virtual void ScrollToTop()
        {
            if (!IsActive)
            {
                return;
            }
            _scrollViewer?.ScrollToVerticalOffsetWithAnimation(0, 500);
        }

        /// <summary>
        /// This method is called whenever the <see cref="_scrollViewer"/> calls the <see cref="DynamicScrollViewer.DynamicScrollViewer.OnScrollCommand"/> command.<br></br>
        /// This base implementation does the following: <br></br>
        /// - set the <see cref="_scrollViewer"/> property if null, <br></br>
        /// - save the new <see cref="OnScrollEvent.VerticalOffset"/> in the <see cref="State"/>, <br></br>
        /// - update the <see cref="AppBar.CanScrollToTop"/> property (e.VerticalOffset > _scrollViewer.ViewportHeight)
        /// </summary>
        /// <param name="e"></param>
        public virtual void OnScrollEvent(OnScrollEvent e)
        {
            if(!IsActive)
            {
                return;
            }

            _scrollViewer = e.Sender;
            OnPropertyChanged(nameof(CanScroll));
            // save the scroll offset
            State.ScrollOffset = e.VerticalOffset;
            
            AppBar.CanScrollToTop = e.VerticalOffset > _scrollViewer.ViewportHeight * 0.8;
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
            if (!IsActive)
            {
                return;
            }
            UpdateAppBarStyle();
            if(AppBar != null)
            {
                AppBar.CanScrollToTop = false;
            }
        }

        public virtual void Update(BaseModel parameter = null)
        {

        }

        public virtual void OnStateChanged()
        {
            //OnPropertyChanged(nameof(State));
        }
    }
}
