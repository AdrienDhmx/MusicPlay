using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicScrollViewer;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Models;
using MusicPlayUI.Core.Services;

namespace MusicPlayUI.MVVM.ViewModels
{
    public abstract class LibraryViewModel : ViewModel
    {
        public LibraryNavigationState LibraryState
        {
            get
            {
                if (base.State is LibraryNavigationState libraryNavigationState)
                    return libraryNavigationState;
                LibraryNavigationState navigationState = new LibraryNavigationState(base.State);
                base.State = navigationState;
                return navigationState;
            }
            set => base.State = value;
        }

        private int _totalFilteredItems = 0;
        private int _totalItemCount = 0;

        public int TotalFilteredItems
        {
            get => _totalFilteredItems;
            set => SetField(ref _totalFilteredItems, value);
        }

        public int TotalItemCount
        {
            get => _totalItemCount;
            set => SetField(ref _totalItemCount, value);
        }

        public LibraryViewModel()
        {
        }

        internal virtual (bool, int, int) CanLoadNewItems(OnScrollEvent onScrollEvent)
        {
            if (onScrollEvent == null)
                return (false, 0, 0);

            if (!onScrollEvent.IsScrollingVertically || !onScrollEvent.IsScrollingForward)
                return (false, 0, 0);

            double offsetThreshold = onScrollEvent.Sender.ScrollableHeight - 200;
            if (onScrollEvent.VerticalOffset > offsetThreshold)
            {
                int startIndex = LibraryState.Page * LibraryState.ItemPerPage;
                LibraryState.Page++;
                int endIndex = LibraryState.Page * LibraryState.ItemPerPage;

                if (endIndex > TotalFilteredItems)
                {
                    endIndex = TotalFilteredItems;
                }

                if (startIndex >= endIndex) // all the data is already loaded
                {
                    return (false, 0, 0);
                }
                return (true, startIndex, endIndex);
            }
            return (false, 0, 0);
        }

        public override void Dispose()
        {

        }

        public override void UpdateAppBarStyle()
        {
            AppBar.Reset();
            AppBar.BackgroundOpacity = 0.05;
            AppBar.Background = AppTheme.Palette.PrimaryContainer;
        }
    }
}
