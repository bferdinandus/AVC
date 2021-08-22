using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using AVC.Core.Events;
using AVC.Core.Models;
using AVC.Core.Services;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace AVC.UI.ViewModels
{
    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public class DeviceControlsViewModel : BindableBase, IDisposable
    {
        private readonly ILogger<DeviceControlsViewModel> _logger;
        private readonly IAudioService _audioService;

        private AudioDeviceModel _selectedDevice;
        private readonly ObservableCollection<AudioDeviceModel> _devices;

        private int _deviceVolume;

        public int DeviceVolume {
            get => _deviceVolume;
            set => SetProperty(ref _deviceVolume, value);
        }

        public ObservableCollection<AudioDeviceModel> Devices {
            get => _devices;
            private init => SetProperty(ref _devices, value);
        }

        public AudioDeviceModel SelectedDevice {
            get => _selectedDevice;
            set => SetProperty(ref _selectedDevice, value);
        }

        public DelegateCommand<SelectionChangedEventArgs> DeviceSelectedCommand { get; private set; }
        public DelegateCommand<RoutedPropertyChangedEventArgs<double>> VolumeChangedCommand { get; private set; }

        public DeviceControlsViewModel(IAudioService audioService,
                                       IEventAggregator eventAggregator,
                                       ILogger<DeviceControlsViewModel> logger)
        {
            _logger = logger;
            _audioService = audioService;

            Devices = new ObservableCollection<AudioDeviceModel>(_audioService.GetActiveOutputDevices());
            SelectedDevice = Devices.Single(s => s.Selected);
            DeviceVolume = SelectedDevice.Volume;

            DeviceSelectedCommand = new DelegateCommand<SelectionChangedEventArgs>(ChangeOutputDevice);
            VolumeChangedCommand = new DelegateCommand<RoutedPropertyChangedEventArgs<double>>(OnVolumeChanged);

            eventAggregator.GetEvent<DeviceUpdateEvent>().Subscribe(OnDeviceUpdate);
        }

        private void OnVolumeChanged(RoutedPropertyChangedEventArgs<double> obj)
        {
            _audioService.SetDeviceVolume(SelectedDevice.Id, (int) obj.NewValue);
        }

        private void OnDeviceUpdate(DeviceUpdateMessage deviceUpdateMessage)
        {
            DeviceVolume = deviceUpdateMessage.Volume;
        }

        private void ChangeOutputDevice(SelectionChangedEventArgs obj)
        {
            if (obj.AddedItems.Count == 0) {
                return;
            }

            if (obj.AddedItems[0] is not AudioDeviceModel device) {
                return;
            }

            DeviceVolume = device.Volume;
            _audioService.SelectDeviceById(device.Id);
        }

        public void Dispose()
        {
            _logger.LogTrace("{Function}()", nameof(Dispose));
        }
    }
}