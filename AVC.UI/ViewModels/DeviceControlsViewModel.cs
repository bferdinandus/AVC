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
        private AudioDeviceModel _selectedDevice;
        private int _deviceVolume;

        public ObservableCollection<AudioDeviceModel> Devices { get; } = new();

        public AudioDeviceModel SelectedDevice {
            get => _selectedDevice;
            set => SetProperty(ref _selectedDevice, value);
        }

        public int DeviceVolume {
            get => _deviceVolume;
            set => SetProperty(ref _deviceVolume, value);
        }

        /*
         * Commands
         */
        public DelegateCommand<SelectionChangedEventArgs> DeviceSelectionChangedCommand { get; }
        public DelegateCommand<RoutedPropertyChangedEventArgs<double>> VolumeChangedCommand { get; }
        public DelegateCommand NextDeviceCommand { get; }
        public DelegateCommand RefreshDevicesCommand { get; }

        /*
         * Constructor
         */
        public DeviceControlsViewModel(IAudioService audioService,
                                       IEventAggregator eventAggregator,
                                       ILogger<DeviceControlsViewModel> logger)
        {
            _logger = logger;
            _audioService = audioService;
            _eventAggregator = eventAggregator;

            UpdateDevices();
            UpdateSelectedDevice();

            DeviceSelectionChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(OnDeviceSelectionChangedCommand);
            VolumeChangedCommand = new DelegateCommand<RoutedPropertyChangedEventArgs<double>>(OnVolumeChangedCommand);
            NextDeviceCommand = new DelegateCommand(OnNextDeviceCommand);
            RefreshDevicesCommand = new DelegateCommand(OnRefreshDevicesCommand);

            eventAggregator.GetEvent<DeviceUpdateEvent>().Subscribe(OnDeviceUpdateEvent);
            eventAggregator.GetEvent<DeviceChangedEvent>().Subscribe(OnDeviceChangedEvent, ThreadOption.UIThread);
        }

        /*
         * Commands functions
         */
        private void OnVolumeChangedCommand(RoutedPropertyChangedEventArgs<double> obj)
        {
            if ((DateTime.Now.Ticks - _lastDeviceUpdateEventReceived) / TimeSpan.TicksPerMillisecond < 50) {
                // block outgoing event x milliseconds after the last received DeviceUpdateEvent publish
                _logger.LogDebug("Blocked {0}", nameof(UiDeviceUpdateEvent));

                return;
            }

            _logger.LogDebug("new volume from slider {0}", obj.NewValue);

            _eventAggregator.GetEvent<UiDeviceUpdateEvent>().Publish(new UiDeviceUpdateMessage { Id = SelectedDevice.Id, Volume = (int) obj.NewValue });
        }

        private void OnDeviceSelectionChangedCommand(SelectionChangedEventArgs args)
        {
            if (args.AddedItems.Count == 0 || args.AddedItems[0] is not AudioDeviceModel addedDeviceModel) {
                return;
            }

            DeviceVolume = addedDeviceModel.Volume;
            _audioService.SelectDeviceById(addedDeviceModel.Id);
        }

        private void OnNextDeviceCommand()
        {
            _audioService.NextDevice();
            UpdateSelectedDevice();
        }

        private void OnRefreshDevicesCommand()
        {
            UpdateDevices(true);
            UpdateSelectedDevice();
        }

        /*
         * Events functions
         */
        private void OnDeviceUpdateEvent(DeviceUpdateMessage message)
        {
            if (DeviceVolume == message.Volume) {
                return;
            }

            _logger.LogDebug("new volume from device {0}", message.Volume);

            _lastDeviceUpdateEventReceived = DateTime.Now.Ticks;
            DeviceVolume = message.Volume;
        }

        private void OnDeviceChangedEvent()
        {
            UpdateDevices();
            UpdateSelectedDevice();
        }

        /*
         * Private functions
         */
        private void UpdateDevices(bool forceRefresh = false)
        {
            Devices.Clear();
            foreach (AudioDeviceModel deviceModel in _audioService.GetActiveOutputDevices(forceRefresh)) {
                Devices.Add(deviceModel);
            }
        }

        private void UpdateSelectedDevice()
        {
            SelectedDevice = Devices.Single(s => s.Selected);
            DeviceVolume = SelectedDevice.Volume;
        }

        public void Dispose()
        {
            _logger.LogTrace("{Function}()", nameof(Dispose));
        }
    }
}