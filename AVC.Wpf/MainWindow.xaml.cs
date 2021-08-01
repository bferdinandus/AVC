using System.Runtime.CompilerServices;
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

        public VolumeSliderViewModel ViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();

            _scope = AppServices.Instance.ServiceProvider.CreateScope();
            Closed += (sender, e) => _scope.Dispose();

            ILogger<MainWindow> logger = _scope.ServiceProvider.GetRequiredService<ILogger<MainWindow>>();
            logger.LogInformation("Main Window Starting");

            ViewModel = _scope.ServiceProvider.GetRequiredService<VolumeSliderViewModel>();

            DataContext = this;
        }

    }
}