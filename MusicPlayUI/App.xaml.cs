using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AudioHandler;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Windows;
using MusicPlayUI.MVVM.Views.Windows;
using MusicPlayUI.Core.Factories;
using MusicFilesProcessor;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Enums;
using System.Diagnostics;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlay.Language;
using MessageControl;
using MusicPlayUI.Core.Commands;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Windows.Input;
using FilesProcessor;
using AudioHandler.Models;
using MusicPlay.Database.DatabaseAccess;
using MusicPlay.Database.Helpers;
using LastFmNamespace.Interfaces;
using LastFmNamespace;
using MusicFilesProcessor.Helpers;
using Microsoft.EntityFrameworkCore;
using MusicPlayUI.MVVM.ViewModels.AppBars;
using MusicPlayUI.Controls;

namespace MusicPlayUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly string defaultImage = "Resources\\DefaultImage.png";
        public static readonly string defaultArtistImage = "Resources\\DefaultArtistImage.jpg";

        public static IAppState State { get; private set; }
        public static ShortcutsManager ShortcutsManager { get; private set; }
        public static LastFm LastFm { get; private set; }
        private const string LastFmApiKey = "5b481c2ba7326b31c2210dcd639bf8ac";

        private IServiceProvider _services;
        private IQueueService _queueService;
        protected override async void OnStartup(StartupEventArgs e)
        {
            App.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;

            // register all the services, views, windows and viewmodels
            var builder = new HostBuilder().Register();
            var host = builder.Build();
            using var scope = host.Services.CreateScope();
            _services = scope.ServiceProvider;

            State = _services.GetService<IAppState>();

            try
            {
                { // make sure we have a database and that the foreign keys are enabled
                    DatabaseContext context = new DatabaseContext();
                    context.Database.EnsureCreated();
                    context.Database.ExecuteSqlRaw("PRAGMA foreign_keys = ON");
                }

                ConfigurationService.Init();

                // init language
                LanguageService.SetLanguage(((SettingsValueEnum)ConfigurationService.GetPreference(SettingsEnum.Language)).GetLanguageCulture());

                // init app theme
                AppTheme.InitializeAppTheme();
                MessageFactory.RegisterMessagesStyles();

                AsyncImage.LoadResourceImage(defaultArtistImage);
                AsyncImage.LoadResourceImage(defaultImage);

                // create and show main window
                MainWindow window = _services.GetRequiredService<MainWindow>();

                // init shortcuts services with this window
                ShortcutsManager = new(_services.GetRequiredService<ICommandsManager>(), window);
                // init the appBar
                State.SetAppBar(_services.GetRequiredService<AppBar>());
                // show the window
                window.Show();

                // init LastFm Services
                LastFm = new(LastFmApiKey, "MusicPlay", "3.0.1", ConnectivityHelper.Instance);

                // check for missing tracks
                int deletedTracks = await ImportMusicLibrary.CheckTrackPaths();
                if (deletedTracks > 0)
                {
                    MessageHelper.PublishMessage(MessageFactory.TrackDeleted(deletedTracks));
                }

                // init playback service
                IAudioPlayback audioPlayback = _services.GetService<IAudioPlayback>();

                // init Equalizer and apply preset if enabled
                audioPlayback.EQManager.Enabled = ConfigurationService.GetPreference(SettingsEnum.EqualizerEnabled) == 1;
                int presetId = ConfigurationService.GetPreference(SettingsEnum.EqualizerPreset);
                if(presetId >= 0) 
                {
                    audioPlayback.EQManager.ApplyPreset(EQPreset.Get(presetId));
                }
                else
                {
                    audioPlayback.EQManager.ApplyPreset(EQModelsFactory.GetPreMadePreset(presetId));
                }

                // init queue services
                _queueService = _services.GetRequiredService<IQueueService>();

                // init the storage settings and scan for new tracks
                StorageService.Instance.FileImported += TrackImported;
                //await Task.Run(() => StorageService.Instance.ScanFolders(true));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in getting/showing MainWindow: " + ex);
                throw;
            }
        }

        private void TrackImported(int newTrackCount)
        {
            if (newTrackCount > 0)
            {
                Current.Dispatcher.Invoke(() =>
                {
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateSuccessMessage($"{newTrackCount} new track(s) imported !"));
                });
            }
            else
            {
                Current.Dispatcher.Invoke(() =>
                {
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateWarningMessage($"No new track found."));
                });
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_queueService.Queue.Tracks.IsNullOrEmpty())
                return;

            try
            {
                // Save the playing queue in the database
                _queueService.SaveQueue();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in inserting queue " + ex);
                throw;
            }
           
        }
    }
}
