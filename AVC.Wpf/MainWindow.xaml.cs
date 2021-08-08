using System;
using System.Windows;
using AVC.Wpf.MVVM.ViewModel;
using Microsoft.Extensions.Logging;

namespace AVC.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ILogger _logger;

        public VolumeSliderViewModel ViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        public MainWindow(ILogger<MainWindow> logger, VolumeSliderViewModel viewModel) : this()
        {
            _logger = logger;
            ViewModel = viewModel;
        }

        protected override void OnClosed(EventArgs e)
        {
            _logger.LogInformation($"{nameof(MainWindow)}.OnClosed()");

            base.OnClosed(e);
        }
    }
}