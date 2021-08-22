/*using System;
using System.Collections.ObjectModel;
using System.Linq;
using AVC.Core.Models;
using AVC.Core.PubSubMessages;
using AVC.Core.Services;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Mvvm;
using PubSubNET;

namespace AVC.Wpf.MVVM.ViewModels
{
    public class OldMainWindowViewModel : BindableBase, IDisposable
    {
        private readonly IAudioService _audioService;
        private readonly ILogger<OldMainWindowViewModel> _logger;
        private bool _doSendMessage = true;

        /*
         * AutoProperties
         #1#
        public ObservableCollection<AudioDeviceModel> AudioDevices { get; } = new();
        public ObservableCollection<AudioSessionModel> AudioSessions { get; } = new();

        /*
         * Full properties
         #1#
        private int _deviceVolumeSliderValue;

        public int DeviceVolumeSliderValue {
            get => _deviceVolumeSliderValue;
            set => SetProperty(ref _deviceVolumeSliderValue, value);
        }

        private Guid _deviceSelectionComboBoxSelectedValue;

        public Guid DeviceSelectionComboBoxSelectedValue {
            get => _deviceSelectionComboBoxSelectedValue;
            set => SetProperty(ref _deviceSelectionComboBoxSelectedValue, value);
        }

        private int _appVolumeSlider1Value;

        public int AppVolumeSlider1Value {
            get => _appVolumeSlider1Value;
            set => SetProperty(ref _appVolumeSlider1Value, value);
        }

        private string _appSelectionComboBox1SelectedValue;

        public string AppSelectionComboBox1SelectedValue {
            get => _appSelectionComboBox1SelectedValue;
            set => SetProperty(ref _appSelectionComboBox1SelectedValue, value);
        }

        /*
         * Constructor
         #1#
        public OldMainWindowViewModel(IAudioService audioService,
                                      ILogger<OldMainWindowViewModel> logger)
        {
            _logger = logger;
            _logger.LogTrace("{Class}()", nameof(OldMainWindowViewModel));
            _audioService = audioService;

            AudioDevices.AddRange(_audioService.GetActiveOutputDevices());
            DeviceSelectionComboBoxSelectedValue = AudioDevices.Single(a => a.Selected).Id;
            DeviceVolumeSliderValue = AudioDevices.Single(a => a.Id == DeviceSelectionComboBoxSelectedValue).Volume;

            AudioSessions.AddRange(_audioService.GetAudioSessionsForDevice(DeviceSelectionComboBoxSelectedValue));

            PubSub.Subscribe<OldMainWindowViewModel, AudioServiceDeviceVolumeUpdate>(this, OnAudioServiceDeviceVolumeUpdate);
            PubSub.Subscribe<OldMainWindowViewModel, ArduinoServiceDeviceVolumeUpdate>(this, OnArduinoDeviceVolumeUpdate);
        }

        /*
         * Commands
         #1#

        private DelegateCommand _deviceSelectionComboBoxSelectionChangedCommand;

        public DelegateCommand DeviceSelectionComboBoxSelectionChangedCommand =>
            _deviceSelectionComboBoxSelectionChangedCommand ??= new DelegateCommand(OnDeviceSelectionComboBoxSelectionChanged);

        private void OnDeviceSelectionComboBoxSelectionChanged()
        {
            _logger.LogTrace("{Class}.{Function}()", nameof(OldMainWindowViewModel), nameof(OnDeviceSelectionComboBoxSelectionChanged));
            _audioService.SelectDeviceById(DeviceSelectionComboBoxSelectedValue);

            DeviceVolumeSliderValue = AudioDevices.Single(a => a.Id == DeviceSelectionComboBoxSelectedValue).Volume;

            AudioSessions.Clear();
            AudioSessions.AddRange(_audioService.GetAudioSessionsForDevice(DeviceSelectionComboBoxSelectedValue));
        }

        private DelegateCommand _deviceVolumeSliderValueChangedCommand;

        public DelegateCommand DeviceVolumeSliderValueChangedCommand => _deviceVolumeSliderValueChangedCommand ??= new DelegateCommand(OnDeviceVolumeSliderValueChanged);

        private void OnDeviceVolumeSliderValueChanged()
        {
            _logger.LogDebug("{Class}.{Function}()", nameof(OldMainWindowViewModel), nameof(OnDeviceVolumeSliderValueChanged));
            if (_doSendMessage) {
                PubSub.Publish(new MainWindowDeviceVolumeUpdate(DeviceVolumeSliderValue, AudioDevices.Single(a => a.Id == DeviceSelectionComboBoxSelectedValue).FullName));
            }

            _doSendMessage = true;
        }

        /*
         * Other functions
         #1#

        private void OnAudioServiceDeviceVolumeUpdate(AudioServiceDeviceVolumeUpdate message)
        {
            _logger.LogDebug("{Class}.{Function}()", nameof(OldMainWindowViewModel), nameof(OnAudioServiceDeviceVolumeUpdate));

            _doSendMessage = false;
            DeviceVolumeSliderValue = message.Volume;
        }

        private void OnArduinoDeviceVolumeUpdate(ArduinoServiceDeviceVolumeUpdate message)
        {
            _logger.LogDebug("{Class}.{Function}()", nameof(OldMainWindowViewModel), nameof(OnArduinoDeviceVolumeUpdate));

            _doSendMessage = false;
            DeviceVolumeSliderValue = message.Volume;
        }

        private void AudioDeviceChanged() {}

        private void AppSessionChanged(string value)
        {
            AudioSessionModel session = AudioSessions.Single(a => a.Id == value);
            AppVolumeSlider1Value = session.Volume;
        }

        /*
         * Dispose / destructor
         #1#
        public void Dispose()
        {
            _logger.LogDebug("{Class}.{Function}()", nameof(OldMainWindowViewModel), nameof(Dispose));

            PubSub.Unsubscribe<OldMainWindowViewModel, AudioServiceDeviceVolumeUpdate>(this);
            PubSub.Unsubscribe<OldMainWindowViewModel, ArduinoServiceDeviceVolumeUpdate>(this);
        }
    }
}*/