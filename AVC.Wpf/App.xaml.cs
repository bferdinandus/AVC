using System.Windows;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AVC.Wpf.MVVM.Views;
using AVC.Wpf.Services;
using Microsoft.Extensions.DependencyInjection;
using Prism.DryIoc;
using Prism.Ioc;
using Serilog;

namespace AVC.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        private IArduinoService _arduinoService;
        private IAudioService _audioService;

        public App()
        {

        }

        protected override IContainerExtension CreateContainerExtension()
        {
            Log.Verbose("{Class}.{Function}()", nameof(App), nameof(CreateContainerExtension));
            IContainerExtension containerExtension = base.CreateContainerExtension();

            containerExtension.RegisterServices(services => {
                // Configure Serilog and the sinks at the startup of the app
                Log.Logger = new LoggerConfiguration()
                             .MinimumLevel.Verbose()
                             .Enrich.FromLogContext()
                             .WriteTo.Debug()
                             .CreateLogger();

                services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            });

            return containerExtension;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Log.Verbose("{Class}.{Function}()", nameof(App), nameof(RegisterTypes));

            containerRegistry.RegisterSingleton<IAudioController, CoreAudioController>();
            containerRegistry.RegisterSingleton<IAudioService, AudioService>();
            containerRegistry.RegisterSingleton<IArduinoService, ArduinoService>();
        }

        protected override Window CreateShell()
        {
            Log.Verbose("{Class}.{Function}()", nameof(App), nameof(CreateShell));

            return Container.Resolve<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _arduinoService = Container.Resolve<IArduinoService>();
            _audioService = Container.Resolve<IAudioService>();

            Log.Verbose("{Class}.{Function}()", nameof(App), nameof(OnStartup));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Verbose("{Class}.{Function}()", nameof(App), nameof(OnExit));

            Container.GetContainer().Dispose();

            base.OnExit(e);
        }
    }
}