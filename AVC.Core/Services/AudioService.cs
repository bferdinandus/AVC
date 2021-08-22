﻿using System;
using System.Collections.Generic;
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
    public interface IAudioService
    {
        List<AudioDeviceModel> GetActiveOutputDevices();

        void SelectDeviceById(Guid id);

        public void SetDeviceVolume(Guid id, int value);

        // List<AudioSessionModel> GetAudioSessionsForDevice(Guid id);
        // List<AudioSessionModel> GetAudioSessionsForCurrentDevice();
        // int GetDeviceVolume(Guid id);
        // int GetAudioSessionVolume(string id);
    }

    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public class AudioService : IAudioService, IDisposable
    {
        private readonly List<AudioDeviceModel> _outputDevices = new();
        private readonly List<AudioSessionModel> _audioSessions = new();

        private readonly IAudioController _audioController;
        private readonly IEventAggregator _eventAggregator;

        private readonly ILogger<AudioService> _logger;

        public AudioService(IAudioController audioController,
                            IEventAggregator eventAggregator,
                            ILogger<AudioService> logger)
        {
            _logger = logger;
            _audioController = audioController;
            _eventAggregator = eventAggregator;

            // PubSub.Subscribe<AudioService, ArduinoServiceDeviceVolumeUpdate>(this, OnArduinoDeviceVolumeUpdate);
            //_eventAggregator.GetEvent<UiVolumeChangedEvent>().Subscribe(OnUiVolumeChanged);
        }

        public List<AudioDeviceModel> GetActiveOutputDevices()
        {
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

                    _logger.LogTrace("Device volume changed: {volume}", (int) vc.Device.Volume);

                    deviceModel.Volume = (int) vc.Device.Volume;
                    _eventAggregator.GetEvent<DeviceUpdatedEvent>().Publish(new DeviceUpdateMessage { DeviceName = vc.Device.Name, Volume = (int) vc.Device.Volume });

                    return true;
                });

                _outputDevices.Add(deviceModel);
            }

            return _outputDevices;
        }

        public void SelectDeviceById(Guid id)
        {
            // mark all local models as not selected
            _outputDevices.ForEach(m => m.Selected = false);

            // mark requested model as selected
            _outputDevices.Single(m => m.Id == id).Selected = true;

            // select the output device
            IDevice device = _audioController.GetDevice(id);
            device.SetAsDefault();
            IEnumerable<IDeviceCapability> y = device.GetAllCapabilities();

            foreach (IDeviceCapability deviceCapability in y) {
                _logger.LogInformation("{0}", deviceCapability.GetType());
            }
        }

        public void SetDeviceVolume(Guid id, int value)
        {
            _audioController.GetDevice(id).SetVolumeAsync(value);
        }

        // private void OnArduinoDeviceVolumeUpdate(ArduinoServiceDeviceVolumeUpdate obj)
        // {
        //     _logger.LogDebug("{Class}.{Function}()", nameof(AudioService), nameof(OnArduinoDeviceVolumeUpdate));
        //
        //     SetDeviceVolume(_outputDevices.Single(m => m.Selected).Id, obj.Volume);
        // }
        //
        // private void OnMainWindowDeviceVolumeUpdate(MainWindowDeviceVolumeUpdate obj)
        // {
        //     _logger.LogDebug("{Class}.{Function}()", nameof(AudioService), nameof(OnMainWindowDeviceVolumeUpdate));
        //
        //     SetDeviceVolume(_outputDevices.Single(m => m.Selected).Id, obj.Volume);
        // }

        public void Dispose()
        {
            _logger.LogDebug("{Class}.{Function}()", nameof(AudioService), nameof(Dispose));
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