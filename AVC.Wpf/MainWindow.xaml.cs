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
        public MainWindow(MainViewModel mainViewModel,
                          ILogger<MainWindow> logger)
        {
            logger.LogInformation("Main Window Starting");

            InitializeComponent();
            this.DataContext = mainViewModel;
        }
    }
}