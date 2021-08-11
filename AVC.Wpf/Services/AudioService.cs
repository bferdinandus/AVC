using System;
using System.Collections.Generic;
using System.Linq;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.Observables;
using AudioSwitcher.AudioApi.Session;
using AVC.Wpf.MVVM.Models;
using AVC.Wpf.PubSubMessages;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using PubSubNET;

namespace AVC.Wpf.Services
{
    public interface IAudioService
    {
        List<AudioDeviceModel> GetActiveOutputDevices();
        List<AudioSessionModel> GetAudioSessionsForCurrentDevice();
        List<AudioSessionModel> GetAudioSessionsForDevice(Guid id);
        void SelectDeviceById(Guid id);
        void SetDeviceVolume(Guid id, int value);
        int GetDeviceVolume(Guid id);
        int GetAudioSessionVolume(string id);
        void SetSessionVolume(string id, int value);
    }

    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public class AudioService : IAudioService, IDisposable
    {
        private readonly List<AudioDeviceModel> _outputDevices = new();
        private readonly List<AudioSessionModel> _audioSessions = new();

        private readonly IAudioController _audioController;

        private readonly ILogger<AudioService> _logger;
        private bool _suppressMessage;

        public AudioService(IAudioController audioController,
                            ILogger<AudioService> logger)
        {
            _logger = logger;
            _audioController = audioController;

            PubSub.Subscribe<AudioService, ArduinoDeviceVolumeUpdate>(this, OnArduinoDeviceVolumeUpdate);
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

                // update the model (and somehow tell the viewModel)
                device.VolumeChanged.When(vc => {
                    _logger.LogTrace("Device volume changed: {volume:D}", vc.Device.Volume);

                    if (!_suppressMessage) {
                        deviceModel.Volume = (int) vc.Device.Volume;
                        AudioServiceDeviceVolumeUpdate message = new(deviceModel.Volume);
                        PubSub.Publish(message);
                    } else {
                        _logger.LogTrace("Message suppressed: {type}", nameof(AudioServiceDeviceVolumeUpdate));
                    }

                    _suppressMessage = false;

                    return true;
                });

                _outputDevices.Add(deviceModel);
            }

            return _outputDevices;
        }

        public List<AudioSessionModel> GetAudioSessionsForCurrentDevice()
        {
            return GetAudioSessionsForDevice(_outputDevices.Single(m => m.Selected).Id);
        }

        public List<AudioSessionModel> GetAudioSessionsForDevice(Guid id)
        {
            _audioSessions.Clear();

            IDevice audioDevice = _audioController.GetDevice(id);
            IEnumerable<IAudioSession> sessions = audioDevice.GetCapability<IAudioSessionController>().All();

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

        public void SelectDeviceById(Guid id)
        {
            // mark all local models as not selected
            _outputDevices.ForEach(m => m.Selected = false);

            // mark requested model as selected
            _outputDevices.Single(m => m.Id == id).Selected = true;

            // select the output device
            _audioController.GetDevice(id).SetAsDefault();
        }

        public void SetDeviceVolume(Guid id, int value)
        {
            _suppressMessage = true;

            _audioController.GetDevice(id).SetVolumeAsync(value);
        }

        public int GetDeviceVolume(Guid id)
        {
            return _outputDevices.Single(m => m.Id == id).Volume;
        }

        public int GetAudioSessionVolume(string id)
        {
            return _audioSessions.Single(m => m.Id == id).Volume;
        }

        public void SetSessionVolume(string id, int value)
        {
            IDevice audioDevice = _audioController.GetDevice(_outputDevices.Single(m => m.Selected).Id);
            IAudioSession session = audioDevice.GetCapability<IAudioSessionController>().All().Single(s => s.Id == id);
            session.SetVolumeAsync(value);
        }

        private void SetActiveDeviceVolume(int value)
        {
            SetDeviceVolume(_outputDevices.Single(m => m.Selected).Id, value);
        }

        private void OnArduinoDeviceVolumeUpdate(ArduinoDeviceVolumeUpdate obj)
        {
            SetActiveDeviceVolume(obj.Volume);
        }

        public void Dispose()
        {
            _logger.LogDebug($"{nameof(AudioService)}.Dispose()");
            PubSub.Unsubscribe<AudioService, ArduinoDeviceVolumeUpdate>(this);
        }
    }
}