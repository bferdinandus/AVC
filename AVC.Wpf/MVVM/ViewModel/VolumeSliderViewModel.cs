using System;
using System.Collections.ObjectModel;
using System.Linq;
using AVC.Wpf.Core;
using AVC.Wpf.MVVM.Model;
using AVC.Wpf.Services;
using Microsoft.Extensions.Logging;

namespace AVC.Wpf.MVVM.ViewModel
{
    public class VolumeSliderViewModel : ObservableObject
    {
        private readonly IAudioService _audioService;
        private readonly ILogger<VolumeSliderViewModel> _logger;
        private bool _updateAudioDevice = true;

        private ObservableCollection<AudioDeviceModel> _audioDevices = new();
        private ObservableCollection<AudioSessionModel> _audioSessions = new();
        private int _deviceVolumeSliderValue;
        private Guid _deviceSelectionComboBoxSelectedValue;
        private int _appVolumeSlider1Value;
        private string _appSelectionComboBox1SelectedValue;

        public ObservableCollection<AudioDeviceModel> AudioDevices
        {
            get => _audioDevices;
            set => _audioDevices = value;
        }

        public ObservableCollection<AudioSessionModel> AudioSessions
        {
            get => _audioSessions;
            set => _audioSessions = value;
        }

        public int DeviceVolumeSliderValue
        {
            get => _deviceVolumeSliderValue;
            set
            {
                if (value == _deviceVolumeSliderValue)
                {
                    return;
                }

                _logger.LogInformation($"DeviceVolumeSliderValue old value: {_deviceVolumeSliderValue} new value: {value}");

                _deviceVolumeSliderValue = value;

                if (!_updateAudioDevice)
                {
                    return;
                }

                _logger.LogInformation($"Updating the audioDevice.");
                _audioService.SetDeviceVolume(DeviceSelectionComboBoxSelectedValue, DeviceVolumeSliderValue);
            }
        }

        public Guid DeviceSelectionComboBoxSelectedValue
        {
            get => _deviceSelectionComboBoxSelectedValue;
            set
            {
                _deviceSelectionComboBoxSelectedValue = value;
                AudioDeviceChanged();
            }
        }

        public int AppVolumeSlider1Value
        {
            get => _appVolumeSlider1Value;
            set
            {
                _appVolumeSlider1Value = value;
                _audioService.SetSessionVolume(AppSelectionComboBox1SelectedValue, AppVolumeSlider1Value);
            }
        }

        public string AppSelectionComboBox1SelectedValue
        {
            get => _appSelectionComboBox1SelectedValue;
            set
            {
                _appSelectionComboBox1SelectedValue = value;
                AppSessionChanged(value);
            }
        }

        public VolumeSliderViewModel(IAudioService audioService,
                                     ILogger<VolumeSliderViewModel> logger)
        {
            _logger = logger;
            _audioService = audioService;

            _logger.LogInformation("VolumeSlider Constructor");
            AudioDevices = new ObservableCollection<AudioDeviceModel>(_audioService.GetActiveOutputDevices());
            DeviceSelectionComboBoxSelectedValue = AudioDevices.Single(a => a.Selected).Id;

            //_token = messenger.Subscribe<VolumeUpdateMessage>(OnVolumeUpdate);
        }

        private void OnVolumeUpdate(VolumeUpdateMessage message)
        {
            _updateAudioDevice = message.DoUpdateDeviceVolume;
            DeviceVolumeSliderValue = message.Volume;

            // always reset to true so that when the UI slider updates the device volume gets updated
            _updateAudioDevice = true;
        }

        private void AudioDeviceChanged()
        {
            _audioService.SelectDeviceById(DeviceSelectionComboBoxSelectedValue);
            _updateAudioDevice = false;
            DeviceVolumeSliderValue = AudioDevices.Single(a => a.Id == DeviceSelectionComboBoxSelectedValue).Volume;
            _updateAudioDevice = true;

            AudioSessions = new ObservableCollection<AudioSessionModel>(_audioService.GetAudioSessionsForDevice(DeviceSelectionComboBoxSelectedValue));
        }

        private void AppSessionChanged(string value)
        {
            AudioSessionModel session = AudioSessions.Single(a => a.Id == value);
            AppVolumeSlider1Value = session.Volume;
        }
    }
}