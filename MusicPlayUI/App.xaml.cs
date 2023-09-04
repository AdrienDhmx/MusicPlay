using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AudioHandler;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using MusicPlayUI.MVVM.ViewModels;
using DataBaseConnection.DataAccess;
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

namespace MusicPlayUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly ResourceDictionary appThemeDic = Current.Resources.MergedDictionaries[0];
        public static readonly ResourceDictionary IconDic = new() { Source = new Uri("Resources\\Icons.xaml", UriKind.Relative) };
        public static readonly string defaultImage = "Resources\\DefaultImage.png";
        public static readonly string defaultArtistImage = "Resources\\DefaultArtistImage.jpg";

        public static ShortcutsManager ShortcutsManager { get; private set; }
        private IServiceProvider _services;
        private IQueueService _queueService;
        protected override async void OnStartup(StartupEventArgs e)
        {
            App.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            // register all the services, views, windows and viewmodels
            var builder = new HostBuilder().Register();

            var host = builder.Build();

            using var scope = host.Services.CreateScope();

            _services = scope.ServiceProvider;

            try
            {
                // init language
                LanguageService.SetLanguage(((SettingsValueEnum)ConfigurationService.GetPreference(SettingsEnum.Language)).GetLanguageCulture());

                // init app theme
                AppThemeService.InitializeAppTheme();
                MessageFactory.RegisterMessagesStyles();

                // create and show main window
                MainWindow window = _services.GetRequiredService<MainWindow>();
                window.Show();

                // init shortcuts services and apply 
                ShortcutsManager = new(_services.GetRequiredService<ICommandsManager>(), window);

                // check for missing tracks
                int deletedTracks = await ImportMusicLibrary.CheckTrackPaths();
                if (deletedTracks > 0)
                {
                    MessageHelper.PublishMessage(MessageFactory.TrackDeleted(deletedTracks));
                }

                // init queue services
                _queueService = _services.GetRequiredService<IQueueService>();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in getting/showing MainWindow: " + ex);
                throw;
            }
        }

        protected override async void OnExit(ExitEventArgs e)
        {
            if (_queueService.QueueTracks is null || _queueService.QueueTracks.Count == 0)
                return;

            try
            {
                // Save the playing queue in the database
                await _queueService.SaveQueue();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error in inserting queue " + ex);
                throw;
            }
           
        }
    }
}
