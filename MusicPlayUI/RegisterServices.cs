using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using AudioHandler;
using MusicPlayUI.MVVM.Views.Windows;
using MusicPlayUI.MVVM.ViewModels.ModalViewModels;
using MusicPlayUI.MVVM.ViewModels.PlayerControlViewModels;
using MusicPlayUI.MVVM.ViewModels.PopupViewModels;
using MusicPlayUI.MVVM.ViewModels.SettingsViewModels;
using MusicPlayUI.MVVM.ViewModels;
using MusicPlayUI.Core.Services;
using MusicPlayUI.MVVM.Views;
using MusicPlayUI.MVVM.Views.PlayerControlViews;
using MusicPlayUI.MVVM.Views.PopupViews;
using MusicPlayUI.MVVM.Views.ModalViews;
using MusicPlayUI.MVVM.Views.SettingsViews;
using System.Runtime.CompilerServices;
using System.Windows;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.Core.Commands;

namespace MusicPlayUI
{
    public static class RegisterServices
    {
        public static IServiceProvider service { get; set; }

        public static IHostBuilder Register(this IHostBuilder host)
        {
            return host.ConfigureServices((services) =>
            {
                // func used for navigation
                services.AddSingleton<Func<Type, ViewModel>>(serviceProvider => viewModelType => (ViewModel)serviceProvider.GetRequiredService(viewModelType));
                services.AddSingleton<Func<Type, Window>>(servicesProvider => window => (Window)servicesProvider.GetRequiredService(window));

                // Services
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddSingleton<IQueueService, QueueService>();
                services.AddSingleton<IHistoryServices, HistoryServices>();
                services.AddSingleton<IRadioStationsService, RadioStationsService>();
                services.AddSingleton<IVisualizerParameterStore, VisualizerParameterService>();
                services.AddSingleton<IAudioPlayback, AudioPlayback>();
                services.AddSingleton<IAudioTimeService, AudioTimeService>();
                services.AddSingleton<IModalService, ModalService>();
                services.AddSingleton<IWindowService, WindowService>();
                services.AddSingleton<IPlaylistService, PlaylistService>();
                services.AddSingleton<ICommandsManager, CommandsManager>();

                // Views
                services.AddSingleton<MainMenuView>();
                services.AddSingleton<MainMenuViewModel>();

                services.AddSingleton<PlayerControlView>();
                services.AddSingleton<PlayerControlViewModel>();

                services.AddSingleton<NowPlayingPlayerControlView>();
                services.AddSingleton<NowPlayingPlayerControlViewModel>();

                services.AddTransient<AlbumLibraryView>();
                services.AddTransient<AlbumLibraryViewModel>();

                services.AddTransient<ArtistLibraryView>();
                services.AddTransient<ArtistLibraryViewModel>();

                services.AddTransient<GenreLibraryView>();
                services.AddTransient<GenreLibraryViewModel>();

                services.AddTransient<HomeView>();
                services.AddTransient<HomeViewModel>();

                services.AddTransient<ImportLibraryView>();
                services.AddTransient<ImportLibraryViewModel>();

                services.AddTransient<SettingsView>();
                services.AddTransient<SettingsViewModel>();

                services.AddTransient<AlbumView>();
                services.AddTransient<AlbumViewModel>();

                services.AddTransient<ArtistView>();
                services.AddTransient<ArtistViewModel>();

                services.AddTransient<GenreView>();
                services.AddTransient<GenreViewModel>();

                services.AddTransient<NowPlayingView>();
                services.AddTransient<NowPlayingViewModel>();

                services.AddTransient<QueueView>();
                services.AddTransient<QueueViewModel>();

                services.AddTransient<LyricsView>();
                services.AddTransient<LyricsViewModel>();

                services.AddTransient<TrackInfoView>();
                services.AddTransient<TrackInfoViewModel>();

                services.AddTransient<PlaylistLibraryView>();
                services.AddTransient<PlaylistLibraryViewModel>();

                services.AddTransient<PlaylistView>();
                services.AddTransient<PlaylistViewModel>();

                // Popups
                services.AddTransient<TrackPopupView>();
                services.AddTransient<TrackPopupViewModel>();

                services.AddTransient<AlbumPopupView>();
                services.AddTransient<AlbumPopupViewModel>();

                services.AddTransient<ArtistPopupView>();
                services.AddTransient<ArtistPopupViewModel>();

                services.AddTransient<PlaylistPopupView>();
                services.AddTransient<PlaylistPopupViewModel>();

                services.AddTransient<TagPopupView>();
                services.AddTransient<TagPopupViewModel>();

                services.AddSingleton<QueueDrawerView>();
                services.AddSingleton<QueueDrawerViewModel>();

                // Settings Views
                services.AddTransient<GeneralSettingView>();
                services.AddTransient<GeneralSettingsViewModel>();

                services.AddTransient<AppThemeSettingView>();
                services.AddTransient<AppThemeSettingViewModel>();

                services.AddTransient<LanguageSettingView>();
                services.AddTransient<LanguageSettingViewModel>();

                services.AddTransient<VisualizerSettingView>();
                services.AddTransient<VisualizerSettingViewModel>();

                services.AddTransient<ShortcutSettingView>();
                services.AddTransient<ShortcutSettingViewModel>();

                services.AddTransient<EmptyViewModel>();
                services.AddTransient<EmptyView>();


                // Modals
                services.AddTransient<CreatePlaylistView>();
                services.AddTransient<CreatePlaylistViewModel>();

                services.AddTransient<CreateTagModal>();
                services.AddTransient<CreateTagViewModel>();

                services.AddTransient<ValidationModalView>();
                services.AddTransient<ValidationModalViewModel>();

                services.AddTransient<UpdateShortcutView>();
                services.AddTransient<UpdateShortcutViewModel>();

                // Windows
                services.AddSingleton(provider => new MainWindow()
                {
                    DataContext = provider.GetRequiredService<MainViewModel>()
                });
                services.AddSingleton<MainViewModel>();

                services.AddTransient<VisualizerParametersWindow>(services => new VisualizerParametersWindow()
                {
                    DataContext = services.GetRequiredService<VisualizerSettingViewModel>()
                });

                
                service = services.BuildServiceProvider();
            });
        }
    }
}
