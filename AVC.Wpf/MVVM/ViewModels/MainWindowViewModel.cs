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
        /*
         * Services
         */
        private readonly IAudioService _audioService;
        private readonly ILogger<MainWindowViewModel> _logger;

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

            DeviceSelectionComboBoxSelectionChangedCommand = new DelegateCommand<SelectionChangedEventArgs>(OnDeviceSelectionComboBoxSelectionChanged);

            AudioDevices = new ObservableCollection<AudioDeviceModel>(_audioService.GetActiveOutputDevices());
            DeviceSelectionComboBoxSelectedValue = AudioDevices.Single(a => a.Selected).Id;
            DeviceVolumeSliderValue = AudioDevices.Single(a => a.Id == DeviceSelectionComboBoxSelectedValue).Volume;

            PubSub.Subscribe<MainWindowViewModel, AudioServiceDeviceVolumeUpdate>(this, OnDeviceVolumeUpdate);
        }

        /*
         * Commands
         */
        public DelegateCommand<SelectionChangedEventArgs> DeviceSelectionComboBoxSelectionChangedCommand { get; }

        private void OnDeviceSelectionComboBoxSelectionChanged(SelectionChangedEventArgs obj)
        {
            _audioService.SelectDeviceById(DeviceSelectionComboBoxSelectedValue);

            // _updateAudioDevice = false;
            DeviceVolumeSliderValue = AudioDevices.Single(a => a.Id == DeviceSelectionComboBoxSelectedValue).Volume;

            // _updateAudioDevice = true;

            AudioSessions = new ObservableCollection<AudioSessionModel>(_audioService.GetAudioSessionsForDevice(DeviceSelectionComboBoxSelectedValue));
        }

        private DelegateCommand _deviceVolumeSliderValueChangedCommand;

        public DelegateCommand DeviceVolumeSliderValueChangedCommand => _deviceVolumeSliderValueChangedCommand ??= new DelegateCommand(OnDeviceVolumeSliderValueChanged);

        private void OnDeviceVolumeSliderValueChanged()
        {
            _logger.LogTrace("{Class}.{Function}()", nameof(MainWindowViewModel), nameof(OnDeviceVolumeSliderValueChanged));
            _audioService.SetDeviceVolume(DeviceSelectionComboBoxSelectedValue, DeviceVolumeSliderValue);
        }

        /*
         * Other functions
         */

        private void OnDeviceVolumeUpdate(AudioServiceDeviceVolumeUpdate message)
        {
            _logger.LogInformation($"{nameof(MainWindowViewModel)}.{nameof(OnDeviceVolumeUpdate)}()");

            // _updateAudioDevice = false;
            DeviceVolumeSliderValue = message.Volume;
            // _updateAudioDevice = true;
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
            _logger.LogTrace("{Class}.{Function}()", nameof(MainWindowViewModel), nameof(Dispose));

            PubSub.Unsubscribe<MainWindowViewModel, AudioServiceDeviceVolumeUpdate>(this);
        }
    }
}