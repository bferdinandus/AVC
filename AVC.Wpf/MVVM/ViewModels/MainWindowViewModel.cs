using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using AVC.Wpf.MVVM.Models;
using AVC.Wpf.PubSubMessages;
using AVC.Wpf.Services;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Prism.Mvvm;
using PubSubNET;

namespace AVC.Wpf.MVVM.ViewModels
{
    public class MainWindowViewModel : BindableBase, IDisposable
    {
        private readonly IAudioService _audioService;
        private readonly ILogger<MainWindowViewModel> _logger;
        private bool _doSendMessage = true;

        /*
         * AutoProperties
         */
        public ObservableCollection<AudioDeviceModel> AudioDevices { get; set; } = new();
        public ObservableCollection<AudioSessionModel> AudioSessions { get; set; } = new();

        /*
         * Full properties
         */
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
         */
        public MainWindowViewModel(IAudioService audioService,
                                   ILogger<MainWindowViewModel> logger)
        {
            _logger = logger;
            _logger.LogTrace("{Class}()", nameof(MainWindowViewModel));
            _audioService = audioService;

            AudioDevices = new ObservableCollection<AudioDeviceModel>(_audioService.GetActiveOutputDevices());
            DeviceSelectionComboBoxSelectedValue = AudioDevices.Single(a => a.Selected).Id;
            DeviceVolumeSliderValue = AudioDevices.Single(a => a.Id == DeviceSelectionComboBoxSelectedValue).Volume;

            PubSub.Subscribe<MainWindowViewModel, AudioServiceDeviceVolumeUpdate>(this, OnAudioServiceDeviceVolumeUpdate);
            PubSub.Subscribe<MainWindowViewModel, ArduinoServiceDeviceVolumeUpdate>(this, OnArduinoDeviceVolumeUpdate);
        }

        /*
         * Commands
         */

        private DelegateCommand _deviceSelectionComboBoxSelectionChangedCommand;

        public DelegateCommand DeviceSelectionComboBoxSelectionChangedCommand =>
            _deviceSelectionComboBoxSelectionChangedCommand ??= new DelegateCommand(OnDeviceSelectionComboBoxSelectionChanged);

        private void OnDeviceSelectionComboBoxSelectionChanged()
        {
            _logger.LogTrace("{Class}.{Function}()", nameof(MainWindowViewModel), nameof(OnDeviceSelectionComboBoxSelectionChanged));
            _audioService.SelectDeviceById(DeviceSelectionComboBoxSelectedValue);

            _doSendMessage = false;
            DeviceVolumeSliderValue = AudioDevices.Single(a => a.Id == DeviceSelectionComboBoxSelectedValue).Volume;

            AudioSessions = new ObservableCollection<AudioSessionModel>(_audioService.GetAudioSessionsForDevice(DeviceSelectionComboBoxSelectedValue));
        }

        private DelegateCommand _deviceVolumeSliderValueChangedCommand;

        public DelegateCommand DeviceVolumeSliderValueChangedCommand => _deviceVolumeSliderValueChangedCommand ??= new DelegateCommand(OnDeviceVolumeSliderValueChanged);

        private void OnDeviceVolumeSliderValueChanged()
        {
            _logger.LogDebug("{Class}.{Function}()", nameof(MainWindowViewModel), nameof(OnDeviceVolumeSliderValueChanged));
            if (_doSendMessage) {
                PubSub.Publish(new MainWindowDeviceVolumeUpdate(DeviceVolumeSliderValue));
            }

            _doSendMessage = true;
        }

        /*
         * Other functions
         */

        private void OnAudioServiceDeviceVolumeUpdate(AudioServiceDeviceVolumeUpdate message)
        {
            _logger.LogDebug("{Class}.{Function}()", nameof(MainWindowViewModel), nameof(OnAudioServiceDeviceVolumeUpdate));

            _doSendMessage = false;
            DeviceVolumeSliderValue = message.Volume;
        }

        private void OnArduinoDeviceVolumeUpdate(ArduinoServiceDeviceVolumeUpdate message)
        {
            _logger.LogDebug("{Class}.{Function}()", nameof(MainWindowViewModel), nameof(OnArduinoDeviceVolumeUpdate));

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
         */
        public void Dispose()
        {
            _logger.LogDebug("{Class}.{Function}()", nameof(MainWindowViewModel), nameof(Dispose));

            PubSub.Unsubscribe<MainWindowViewModel, AudioServiceDeviceVolumeUpdate>(this);
            PubSub.Unsubscribe<MainWindowViewModel, ArduinoServiceDeviceVolumeUpdate>(this);
        }
    }
}