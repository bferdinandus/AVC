﻿using Microsoft.Extensions.Logging;
using Prism.Mvvm;

namespace AVC.Wpf.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = " A. V. C. ";

        public string Title {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public MainWindowViewModel(ILogger<MainWindowViewModel> logger)
        {
            logger.LogTrace("{Class}()", nameof(MainWindowViewModel));
        }
    }
}