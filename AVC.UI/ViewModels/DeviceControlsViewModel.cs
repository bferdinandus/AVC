using System.Collections.ObjectModel;
using AudioSwitcher.AudioApi;
using AVC.Core.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Prism.Mvvm;

namespace AVC.UI.ViewModels
{
    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public class DeviceControlsViewModel : BindableBase
    {
        private readonly ILogger<DeviceControlsViewModel> _logger;
        public ObservableCollection<AudioDeviceModel> Devices { get; } = new();

        public DeviceControlsViewModel(ILogger<DeviceControlsViewModel> logger)
        {
            _logger = logger;
            _logger.LogTrace("{Class}()", nameof(DeviceControlsViewModel));

            Devices.Add(new AudioDeviceModel { FullName = "Device 1" });
            Devices.Add(new AudioDeviceModel { FullName = "Device 2" });
            Devices.Add(new AudioDeviceModel { FullName = "Device 3" });
        }
    }
}