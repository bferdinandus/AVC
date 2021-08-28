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
        // services
        private readonly ILogger<DeviceControlsViewModel> _logger;
        private readonly IAudioService _audioService;
        private readonly IEventAggregator _eventAggregator;

        // private variables
        private long _lastDeviceUpdateEventReceived = DateTime.Now.Ticks;

        // UI properties
        private readonly ObservableCollection<AudioDeviceModel> _devices;
        private AudioDeviceModel _selectedDevice;
        private int _deviceVolume;

        public ObservableCollection<AudioDeviceModel> Devices {
            get => _devices;
            private init => SetProperty(ref _devices, value);
        }

        public AudioDeviceModel SelectedDevice {
            get => _selectedDevice;
            set => SetProperty(ref _selectedDevice, value);
        }

        public int DeviceVolume {
            get => _deviceVolume;
            set => SetProperty(ref _deviceVolume, value);
        }

        // commands
        public DelegateCommand<SelectionChangedEventArgs> DeviceSelectionChangedCommand { get; private set; }
        public DelegateCommand<RoutedPropertyChangedEventArgs<double>> VolumeChangedCommand { get; private set; }

        /*
         * constructor
         */
        public DeviceControlsViewModel(IAudioService audioService,
                                       IEventAggregator eventAggregator,
                                       ILogger<DeviceControlsViewModel> logger)
        {
            _logger = logger;
            _audioService = audioService;
            _eventAggregator = eventAggregator;

            Devices = _audioService.GetActiveOutputDevices();
            SelectedDevice = Devices.Single(s => s.Selected);
            DeviceVolume = SelectedDevice.Volume;

            DeviceSelectionChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(OnDeviceSelectionChangedCommand);
            VolumeChangedCommand = new DelegateCommand<RoutedPropertyChangedEventArgs<double>>(OnVolumeChangedCommand);

            eventAggregator.GetEvent<DeviceUpdateEvent>().Subscribe(OnDeviceUpdateEvent);
        }

        private void OnVolumeChangedCommand(RoutedPropertyChangedEventArgs<double> obj)
        {
            if ((DateTime.Now.Ticks - _lastDeviceUpdateEventReceived) / TimeSpan.TicksPerMillisecond < 50) {
                // block outgoing event x milliseconds after the last received DeviceUpdateEvent publish
                _logger.LogDebug("Blocked {0}", nameof(UIDeviceUpdateEvent));

                return;
            }

            _logger.LogDebug("new volume from slider {0}", obj.NewValue);

            _eventAggregator.GetEvent<UIDeviceUpdateEvent>().Publish(new UIDeviceUpdateMessage { Id = SelectedDevice.Id, Volume = (int) obj.NewValue });
        }

        private void OnDeviceSelectionChangedCommand(SelectionChangedEventArgs obj)
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

        private void OnDeviceUpdateEvent(DeviceUpdateMessage message)
        {
            if (DeviceVolume == message.Volume) {
                return;
            }

            _logger.LogDebug("new volume from device {0}", message.Volume);

            _lastDeviceUpdateEventReceived = DateTime.Now.Ticks;
            DeviceVolume = message.Volume;
        }

        public void Dispose()
        {
            _logger.LogTrace("{Function}()", nameof(Dispose));
        }
    }
}