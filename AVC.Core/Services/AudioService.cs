using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.Observables;
using AVC.Core.Events;
using AVC.Core.Models;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Prism.Events;

namespace AVC.Core.Services
{
    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public class AudioService : IAudioService, IDisposable
    {
        // services
        private readonly IAudioController _audioController;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger<AudioService> _logger;

        //private variables
        private readonly ObservableCollection<AudioDeviceModel> _outputDevices = new();
        private readonly List<AudioSessionModel> _audioSessions = new();

        /*
         * constructor
         */
        public AudioService(IAudioController audioController,
                            IEventAggregator eventAggregator,
                            ILogger<AudioService> logger)
        {
            _logger = logger;
            _audioController = audioController;
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<UIDeviceUpdateEvent>().Subscribe(OnUIDeviceUpdateEvent);
            _eventAggregator.GetEvent<ArduinoDeviceUpdateEvent>().Subscribe(OnArduinoDeviceUpdateEvent);
        }

        public ObservableCollection<AudioDeviceModel> GetActiveOutputDevices()
        {
            _outputDevices.Clear();

            IEnumerable<IDevice> devices = _audioController.GetDevices(DeviceType.Playback, DeviceState.Active);

            foreach (IDevice device in devices) {
                AudioDeviceModel deviceModel = new() {
                    Id = device.Id,
                    FullName = device.Name,
                    Selected = device.IsDefaultDevice,
                    Volume = (int) device.Volume,
                    Muted = device.IsMuted,
                    IconPath = device.IconPath
                };

                // update the model and send eventMessage
                device.VolumeChanged.When(vc => {
                    if (deviceModel.Volume == (int) vc.Device.Volume) {
                        return false;
                    }

                    _logger.LogDebug("device volume: {deviceVolume} new volume: {volume}", deviceModel.Volume, (int) vc.Device.Volume);

                    deviceModel.Volume = (int) vc.Device.Volume;
                    _eventAggregator.GetEvent<DeviceUpdateEvent>().Publish(new DeviceUpdateMessage { DeviceName = vc.Device.Name, Volume = deviceModel.Volume });

                    return true;
                });

                _outputDevices.Add(deviceModel);
            }

            return _outputDevices;
        }

        public void SelectDeviceById(Guid id)
        {
            // mark all local models as not selected
            foreach (AudioDeviceModel audioDeviceModel in _outputDevices) {
                audioDeviceModel.Selected = false;
            }

            // mark requested model as selected
            _outputDevices.Single(m => m.Id == id).Selected = true;

            // select the output device
            IDevice device = _audioController.GetDevice(id);
            device.SetAsDefault();
        }

        private void SetDeviceVolume(Guid id, int value)
        {
            _audioController.GetDevice(id).SetVolumeAsync(value);
        }

        public void NextDevice()
        {
            int index = _outputDevices.IndexOf(_outputDevices.Single(d => d.Selected));

            index++;
            if (index >= _outputDevices.Count) {
                index = 0;
            }

            SelectDeviceById(_outputDevices[index].Id);
        }

        // events
        private void OnArduinoDeviceUpdateEvent(ArduinoDeviceUpdateMessage message)
        {
            switch (message.Channel) {
                case 0: // device (master) channel
                    SetDeviceVolume(_outputDevices.Single(m => m.Selected).Id, message.Volume);
                    break;
                case 1: // audion session channel 1
                    break;
                case 2: // audio session channel 2
                    break;
            }
        }

        private void OnUIDeviceUpdateEvent(UIDeviceUpdateMessage message)
        {
            SetDeviceVolume(message.Id, message.Volume);
        }

        public void Dispose()
        {
            _logger.LogTrace("{Function}()", nameof(Dispose));
        }

        /*
         public List<AudioSessionModel> GetAudioSessionsForCurrentDevice()
        {
            return GetAudioSessionsForDevice(_outputDevices.Single(m => m.Selected).Id);
        }

        public List<AudioSessionModel> GetAudioSessionsForDevice(Guid id)
        {
            _audioSessions.Clear();

            IDevice audioDevice = _audioController.GetDevice(id);
            IEnumerable<IAudioSession> sessions = audioDevice.GetCapability<IAudioSessionController>().AllAsync().Result;

            foreach (IAudioSession session in sessions) {
                AudioSessionModel sessionModel = new() {
                    Id = session.Id,
                    DisplayName = session.DisplayName,
                    IconPath = session.IconPath,
                    State = session.SessionState,
                    ProcessId = session.ProcessId,
                    IsSystemSoundsSession = session.IsSystemSession,
                    Volume = (int) session.Volume,
                    Muted = session.IsMuted
                };

                // update the model (and somehow tell the viewModel)
                session.VolumeChanged.When(vc => {
                    sessionModel.Volume = (int) vc.Session.Volume;
                    return true;
                });

                _audioSessions.Add(sessionModel);
            }

            return _audioSessions;
        }

        public int GetAudioSessionVolume(string id)
        {
            return _audioSessions.Single(m => m.Id == id).Volume;
        }

        private void SetSessionVolume(string id, int value)
        {
            IDevice audioDevice = _audioController.GetDevice(_outputDevices.Single(m => m.Selected).Id);
            IAudioSession session = audioDevice.GetCapability<IAudioSessionController>().All().Single(s => s.Id == id);
            session.SetVolumeAsync(value);
        }

         */
    }
}