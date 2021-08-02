using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace AVC.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // private readonly IServiceScope _scope;

        public App()
        {
            // _scope = AppServices.Instance.ServiceProvider.CreateScope();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // _scope.Dispose();
            AppServices.Instance.Dispose();

            base.OnExit(e);
        }
    }
}