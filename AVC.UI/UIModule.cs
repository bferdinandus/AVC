﻿using AVC.Core;
using AVC.UI.Views;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace AVC.UI
{
    public class UiModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly ILogger<UiModule> _logger;

        public UiModule(IRegionManager regionManager,
                        ILogger<UiModule> logger)
        {
            _regionManager = regionManager;
            _logger = logger;

            _logger.LogTrace("{Class}()", nameof(UiModule));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            _logger.LogTrace("{Class}.{Function}()", nameof(UiModule), nameof(RegisterTypes));
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _logger.LogTrace("{Class}.{Function}()", nameof(UiModule), nameof(OnInitialized));

            _regionManager.RegisterViewWithRegion(RegionNames.DeviceControls, typeof(DeviceControls));
        }
    }
}