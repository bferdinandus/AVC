using System;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AVC.Wpf.MVVM.ViewModel;
using AVC.Wpf.Services;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AVC.Wpf
{
    public sealed class AppServices : IDisposable
    {
        public IServiceProvider ServiceProvider { get; }

        private static AppServices _instance;
        private static readonly object InstanceLock = new();

        private static AppServices GetInstance()
        {
            lock (InstanceLock)
            {
                return _instance ??= new AppServices();
            }
        }

        public static AppServices Instance => _instance ?? GetInstance();

        private AppServices()
        {
            ServiceCollection services = new();

            // logging configuration
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Debug()
                .CreateLogger();
            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
            Log.Logger.Verbose("AppServices initializing.");
            services.AddLogging(builder => { builder.AddSerilog(); });

            // view-models (transient)
            services.AddTransient<VolumeSliderViewModel>();
            services.AddTransient<MainWindow>();

            // services (transient)

            // stateful services (scoped)
            services.AddScoped<IAudioController, CoreAudioController>();
            services.AddScoped<IAudioService, AudioService>();
            services.AddScoped<ISerialCommunication, SerialCommunication>();

            // service (singleton)


            ServiceProvider = services.BuildServiceProvider();
        }

        public void Dispose()
        {
            ((ServiceProvider) ServiceProvider)?.Dispose();
        }
    }
}