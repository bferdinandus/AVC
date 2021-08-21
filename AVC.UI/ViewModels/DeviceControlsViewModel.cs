using System.Collections.ObjectModel;
using System.Windows.Controls;
using AVC.Core.Models;
using AVC.Core.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Mvvm;

namespace AVC.UI.ViewModels
{
    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public class DeviceControlsViewModel : BindableBase
    {
        private readonly ILogger<DeviceControlsViewModel> _logger;
        private readonly IAudioService _audioService;

        public ObservableCollection<AudioDeviceModel> Devices { get; } = new();

        private AudioDeviceModel _selectedDevice;

        public AudioDeviceModel SelectedDevice {
            get => _selectedDevice;
            set => SetProperty(ref _selectedDevice, value);
        }

        public DelegateCommand<SelectionChangedEventArgs> DeviceSelectedCommand { get; private set; }

        public DeviceControlsViewModel(ILogger<DeviceControlsViewModel> logger, IAudioService audioService)
        {
            _logger = logger;
            _audioService = audioService;

            _logger.LogTrace("{Class}()", nameof(DeviceControlsViewModel));

            foreach (AudioDeviceModel device in _audioService.GetActiveOutputDevices()) {
                Devices.Add(device);
                if (device.Selected) {
                    SelectedDevice = device;
                }
            }

            DeviceSelectedCommand = new DelegateCommand<SelectionChangedEventArgs>(ChangeOutputDevice);
        }

        private void ChangeOutputDevice(SelectionChangedEventArgs obj)
        {
            if (obj.AddedItems.Count == 0) {
                return;
            }

            AudioDeviceModel device = (AudioDeviceModel) obj.AddedItems[0];

            if (device != null) {
                _audioService.SelectDeviceById(device.Id);
            }
        }
    }
}