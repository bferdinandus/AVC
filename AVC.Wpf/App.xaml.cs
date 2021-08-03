using System.Windows;
using AVC.Wpf.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AVC.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceScope _scope;
        private readonly ILogger _logger;
        private readonly ISerialCommunication _serialComm;

        public App()
        {
            _scope = AppServices.Instance.ServiceProvider.CreateScope();
            _logger = _scope.ServiceProvider.GetService<ILogger<App>>();
            _logger.LogInformation("App()");
            _serialComm = _scope.ServiceProvider.GetService<ISerialCommunication>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _logger.LogInformation($"{nameof(App)}.OnStartup()");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _logger.LogInformation($"{nameof(App)}.OnExit()");
            _scope.Dispose();
            AppServices.Instance.Dispose();
            base.OnExit(e);
        }
    }
}