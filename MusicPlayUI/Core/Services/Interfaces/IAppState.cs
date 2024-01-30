using System;
using System.Collections.Generic;
using MusicPlay.Database.Models;
using MusicPlayUI.Core.Models;
using MusicPlayUI.MVVM.ViewModels;
using MusicPlayUI.MVVM.ViewModels.AppBars;

namespace MusicPlayUI.Core.Services.Interfaces
{
    public interface IAppState
    {
        AppBar AppBar { get; }
        NavigationModel CurrentPopup { get; set; }
        NavigationModel CurrentView { get; set; }
        bool IsFullScreen { get; set; }
        bool IsMenuDrawerOpen { get; set; }
        bool IsQueueDrawerOpen { get; set; }
        bool IsPopupOpen { get; set; }

        bool CanNavigateBack { get; set; }
        bool CanNavigateForward { get; set; }

        event Action FullScreenChanged;
        event Action CurrentViewChanged;

        /// <summary>
        /// Set the app bar
        /// </summary>
        /// <param name="appBar"></param>
        void SetAppBar(AppBar appBar);

        /// <summary>
        /// Navigate to the last page in the Back stack of the history,
        /// and push the current page to the Forward stack of the history
        /// </summary>
        void NavigateBack();

        /// <summary>
        /// Navigate to the previous page that has been pushed forward in the history by calling <see cref="NavigateBack"/>
        /// </summary>
        void NavigateForward();

        /// <summary>
        /// Create a NavigationModel that can be used to navigate
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="state"></param>
        /// <returns></returns>
        NavigationModel CreateNavigationModel<TViewModel>(NavigationState state = null) where TViewModel : ViewModel;

        /// <summary>
        /// Create a NavigationModel with the specified view model type using reflection with an optional parameter.
        /// </summary>
        /// <param name="viewModelType"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        NavigationModel CreateNavigationModel(Type viewModelType, NavigationState state = null);

        /// <summary>
        /// Navigate to the specified view model with the provided state.
        /// </summary>
        /// <typeparam name="TViewModel">The type of view model to navigate to.</typeparam>
        /// <param name="state">The state to initialize the view model with.</param>
        /// <param name="saveView">Determines whether to save the current view model in the navigation history.</param>
        void NavigateTo<TViewModel>(NavigationState state, bool saveView = true) where TViewModel : ViewModel;
        /// <summary>
        /// Navigate to the specified view model with the given parameter in its state.
        /// </summary>
        /// <typeparam name="TViewModel">The type of view model to navigate to.</typeparam>
        /// <param name="parameter">The parameter to pass to the state of the view model.</param>
        /// <param name="saveView">Determines whether to save the current view model in the navigation history.</param>
        void NavigateTo<TViewModel>(BaseModel parameter = null, bool saveView = true) where TViewModel : ViewModel;
        /// <summary>
        /// Navigate to the specified view model type using reflection with an optional parameter.
        /// </summary>
        /// <param name="viewModelType">The type of view model to navigate to.</param>
        /// <param name="parameter">The parameter to pass to the state of the view model (optional).</param>
        /// <param name="saveView">Determines whether to save the current view model in the navigation history.</param>
        void NavigateTo(Type viewModelType, BaseModel parameter = null, bool saveView = true);
        /// <summary>
        /// Navigate to the specified main view model and child view model with the given states.
        /// </summary>
        /// <typeparam name="TViewModel">The type of main view model to navigate to.</typeparam>
        /// <typeparam name="TChildViewModel">The type of child view model to navigate to.</typeparam>
        /// <param name="state">The state to initialize the main view model with.</param>
        /// <param name="childState">The state to initialize the child view model with.</param>
        /// <param name="saveView">Determines whether to save the current view model in the navigation history.</param>
        void NavigateTo<TViewModel, TChildViewModel>(NavigationState state = null, NavigationState childState = null, bool saveView = true)
            where TViewModel : ViewModel
            where TChildViewModel : ViewModel;

        /// <summary>
        /// Call <see cref="ViewModel.Update(BaseModel)"/> if the current view is any of those Type types
        /// </summary>
        /// <param name="viewModels">The types to allow to update</param>
        /// <returns>If the <see cref="CurrentView"/> has been updated</returns>
        public bool UpdateCurrentViewIfIs(List<Type> viewModels);

        /// <summary>
        /// Open a popup with the specified view model and state.
        /// </summary>
        /// <typeparam name="TPopupViewModel">The type of view model for the popup.</typeparam>
        /// <param name="state">The state to initialize the popup view model with.</param>
        void OpenPopup<TPopupViewModel>(NavigationState state = null) where TPopupViewModel : ViewModel;
        /// <summary>
        /// Open a popup with the specified view model and optional parameter in its state.
        /// </summary>
        /// <typeparam name="TPopupViewModel">The type of view model for the popup.</typeparam>
        /// <param name="parameter">The parameter to pass to the state of the popup view model (optional).</param>
        void OpenPopup<TPopupViewModel>(BaseModel parameter = null) where TPopupViewModel : ViewModel;
        /// <summary>
        /// Close the currently open popup.
        /// </summary>
        void ClosePopup();
        /// <summary>
        /// Toggle the application between full-screen and windowed mode.
        /// </summary>
        void ToggleFullScreen();

        /// <summary>
        /// Toggle the visibility of the menu drawer.
        /// </summary>
        void ToggleMenuDrawer();

        /// <summary>
        /// Toggle the visibility of the queue drawer.
        /// </summary>
        void ToggleQueueDrawer();
    }
}