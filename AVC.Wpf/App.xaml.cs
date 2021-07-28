using System;
using System.Windows;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AVC.Wpf.MVVM.ViewModel;
using AVC.Wpf.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AVC.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider ServiceProvider { get; private set; }
        //public IConfiguration Configuration { get; private set; }

        public App()
        {
        }

        protected override void OnStartup(StartupEventArgs eventArgs)
        {
            // IConfigurationBuilder builder = new ConfigurationBuilder()
            //     .SetBasePath(Directory.GetCurrentDirectory())
            //     .AddJsonFile("appsettings.json", false, true);
            // Configuration = builder.Build();

            ServiceCollection serviceCollection = new();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            MainWindow mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Logger.Verbose("Application OnExit");
            try
            {

            }
            finally
            {
                base.OnExit(e);
            }
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IAudioController, CoreAudioController>();
            serviceCollection.AddSingleton<IAudioService, AudioService>();

            serviceCollection.AddTransient<VolumeSliderViewModel>();
            serviceCollection.AddTransient<MainViewModel>();
            serviceCollection.AddTransient<MainWindow>();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Debug()
                .CreateLogger();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
            Log.Logger.Verbose("Application Starting");

            serviceCollection.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
        }
    }
}