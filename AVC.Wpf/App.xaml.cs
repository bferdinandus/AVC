using System.Windows;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AVC.Wpf.MVVM.Views;
using AVC.Wpf.Services;
using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
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
        protected override IContainerExtension CreateContainerExtension()
        {
            // https://www.andicode.com/prism/wpf/logging/2021/05/21/Logging-In-Prism.html

            // Configure Serilog and the sinks at the startup of the app
            Log.Logger = new LoggerConfiguration()
                         .MinimumLevel.Verbose()
                         .Enrich.FromLogContext()
                         .WriteTo.Debug()
                         .CreateLogger();

            Log.Verbose("{Class}.{Function}()", nameof(App), nameof(CreateContainerExtension));
            ServiceCollection serviceCollection = new();
            serviceCollection.AddLogging(loggingBuilder =>
                                             loggingBuilder.AddSerilog(dispose: true));

            return new DryIocContainerExtension(new Container(CreateContainerRules()).WithDependencyInjectionAdapter(serviceCollection));
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            Log.Verbose("{Class}.{Function}()", nameof(App), nameof(RegisterTypes));

            containerRegistry.RegisterSingleton<IAudioController, CoreAudioController>();
            containerRegistry.RegisterSingleton<IAudioService, AudioService>();
            containerRegistry.RegisterSingleton<ISerialCommunication, SerialCommunication>();
        }

        protected override Window CreateShell()
        {
            Log.Verbose("{Class}.{Function}()", nameof(App), nameof(CreateShell));

            return Container.Resolve<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

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