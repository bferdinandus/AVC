using System;
using System.Windows;
using AVC.Wpf.MVVM.ViewModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AVC.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IServiceScope _scope;
        private readonly ILogger _logger;

        public VolumeSliderViewModel ViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();

            _scope = AppServices.Instance.ServiceProvider.CreateScope();

            _logger = _scope.ServiceProvider.GetRequiredService<ILogger<MainWindow>>();
            _logger.LogInformation($"{nameof(MainWindow)}()");

            ViewModel = _scope.ServiceProvider.GetRequiredService<VolumeSliderViewModel>();

            DataContext = this;
        }

        protected override void OnClosed(EventArgs e)
        {
            _logger.LogInformation($"{nameof(MainWindow)}.OnClosed()");
            _scope.Dispose();
            base.OnClosed(e);
        }
    }
}