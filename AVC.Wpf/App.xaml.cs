using System.Threading.Tasks;
using System.Windows;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AVC.Core.Services;
using AVC.UI;
using AVC.Wpf.Views;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Modularity;
using Serilog;

namespace AVC.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        [UsedImplicitly]
        private IArduinoService _arduinoService;

        [UsedImplicitly]
        private IAudioService _audioService;

        // public App() {}

        protected override void OnStartup(StartupEventArgs e)
        {
            //initialize the splash screen and set it as the application main window
            SplashScreenWindow splashScreen = new();
            MainWindow = splashScreen;
            splashScreen.Show();

            // base startup calls the functions below in order
            base.OnStartup(e);

            Log.Verbose("{Class}.{Function}()", nameof(App), nameof(OnStartup));

            Log.Information("Initializing services in OnStartup()");

            //in order to ensure the UI stays responsive, we need to
            //do the work on a different thread
            Task.Factory.StartNew(() => {
                _arduinoService = Container.Resolve<IArduinoService>();
                _audioService = Container.Resolve<IAudioService>();

                //since we're not on the UI thread
                //once we're done we need to use the Dispatcher
                //to create and show the main window
                Dispatcher.Invoke(() => {
                    //initialize the main window, set it as the application main window
                    //and close the splash screen

                    Window shell = Container.Resolve<MainWindow>();
                    MainWindow = shell;
                    shell.Show();
                    splashScreen.Close();
                });
            });
        }

        protected override IContainerExtension CreateContainerExtension()
        {
            IContainerExtension containerExtension = base.CreateContainerExtension();

            containerExtension.RegisterServices(services => {
                // Configure Serilog and the sinks at the startup of the app
                const string consoleOutputTemplate =
                    @"[{Timestamp:HH:mm:ss.fff}] [{Level:u3}] [{SourceContext}] [{RequestId}]{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}";

                Log.Logger = new LoggerConfiguration()
                             .MinimumLevel.Debug()
                             .Enrich.FromLogContext()

                             //.Enrich.WithMachineName()
                             //.Enrich.WithProperty("Assembly", Assembly.GetEntryAssembly()?.GetName().Name)
                             .WriteTo.Debug(outputTemplate: consoleOutputTemplate)
                             .CreateLogger();

                services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            });
            Log.Verbose("{Class}.{Function}()", nameof(App), nameof(CreateContainerExtension));

            return containerExtension;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Log.Verbose("{Class}.{Function}()", nameof(App), nameof(RegisterTypes));

            containerRegistry.RegisterSingleton<IAudioController, CoreAudioController>();
            containerRegistry.RegisterSingleton<IAudioService, AudioService>();
            containerRegistry.RegisterSingleton<IArduinoService, ArduinoService>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);

            Log.Verbose("{Class}.{Function}()", nameof(App), nameof(ConfigureModuleCatalog));

            moduleCatalog.AddModule<UiModule>();
        }

        protected override Window CreateShell()
        {
            Log.Verbose("{Class}.{Function}()", nameof(App), nameof(CreateShell));

            //return Container.Resolve<MainWindow>();

            return null;
        }

        protected override void OnInitialized()
        {
            // base function opens the main window
            base.OnInitialized();

            Log.Verbose("{Class}.{Function}()", nameof(App), nameof(OnInitialized));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Log.Verbose("{Class}.{Function}()", nameof(App), nameof(OnExit));

            Container.GetContainer().Dispose();

            base.OnExit(e);
        }
    }
}