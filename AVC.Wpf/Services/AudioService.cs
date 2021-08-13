using System;
using System.Collections.Generic;
using System.Linq;
using AVC.Wpf.MVVM.Models;
using AVC.Wpf.PubSubMessages;
using CoreAudio;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using PubSubNET;

namespace AVC.Wpf.Services
{
    public interface IAudioService
    {
        List<AudioDeviceModel> GetActiveOutputDevices();
        List<AudioSessionModel> GetAudioSessionsForDevice(string id);
        void SelectDeviceById(string id);
    }

    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public class AudioService : IAudioService, IDisposable
    {
        private readonly MMDeviceEnumerator _deviceEnumerator;
        private readonly ILogger<AudioService> _logger;

        private readonly List<AudioDeviceModel> _outputDevices = new();
        private readonly List<AudioSessionModel> _audioSessions = new();

        private bool _doSendMessage = true;

        public AudioService(MMDeviceEnumerator deviceEnumerator,
                            ILogger<AudioService> logger)
        {
            _deviceEnumerator = deviceEnumerator;
            _logger = logger;

            PubSub.Subscribe<AudioService, ArduinoServiceDeviceVolumeUpdate>(this, OnArduinoDeviceVolumeUpdate);
            PubSub.Subscribe<AudioService, MainWindowDeviceVolumeUpdate>(this, OnMainWindowDeviceVolumeUpdate);
        }

        public List<AudioDeviceModel> GetActiveOutputDevices()
        {
            MMDeviceCollection devices = _deviceEnumerator.EnumerateAudioEndPoints(EDataFlow.eRender, DEVICE_STATE.DEVICE_STATE_ACTIVE);

            foreach (MMDevice device in devices) {
                AudioDeviceModel deviceModel = new() {
                    Id = device.ID,
                    Selected = device.Selected,
                    Volume = (int) (device.AudioEndpointVolume.MasterVolumeLevelScalar * 100),
                    Muted = device.AudioEndpointVolume.Mute,
                    IconPath = "",
                    FullName = device.Properties.Contains(PKEY.PKEY_Device_DeviceDesc)
                        ? (string) device.Properties[PKEY.PKEY_Device_DeviceDesc].Value
                        : device.FriendlyName
                };

                // for (int i = 0; i < device.Properties.Count; i++) {
                //     PropertyStoreProperty x = device.Properties[i];
                //     _logger.LogInformation("Property {p1}, {p2} Value {v}", x.Key.fmtid, x.Key.pid, x.Value);
                // }

                // device.AudioEndpointVolume.OnVolumeNotification += delegate(AudioVolumeNotificationData data) {
                //     int newVolume = (int) (data.MasterVolume * 100);
                //     _logger.LogTrace("Device volume changed: {volume}", newVolume);
                //
                //     if (_doSendMessage) {
                //         PubSub.Publish(new AudioServiceDeviceVolumeUpdate(newVolume));
                //     }
                //
                //     _doSendMessage = true;
                // };

                _outputDevices.Add(deviceModel);
            }

            return _outputDevices;
        }

        public List<AudioSessionModel> GetAudioSessionsForDevice(string id)
        {
            _audioSessions.Clear();

            MMDevice device = _deviceEnumerator.GetDevice(id);
            device.AudioSessionManager2.RefreshSessions();
            SessionCollection sessions = device.AudioSessionManager2.Sessions;

            foreach (AudioSessionControl2 session in sessions) {
                AudioSessionModel sessionModel = new() {
                    Id = session.GetSessionIdentifier,
                    DisplayName = session.DisplayName,
                    IconPath = session.IconPath,
                    State = session.State,
                    ProcessId = session.GetProcessID,
                    IsSystemSoundsSession = session.IsSystemSoundsSession,
                    Volume = (int) (session.SimpleAudioVolume.MasterVolume * 100),
                    Muted = session.SimpleAudioVolume.Mute
                };

                // update the model (and somehow tell the viewModel)
                // session.OnSimpleVolumeChanged += delegate(object sender, float volume, bool mute) {
                //     sessionModel.Volume = (int) volume * 100;
                //     sessionModel.Muted = mute;
                // };

                _audioSessions.Add(sessionModel);
            }

            return _audioSessions;
        }

        public void SelectDeviceById(string id)
        {
            // mark all local models as not selected
            _outputDevices.ForEach(m => m.Selected = false);

            // mark requested model as selected
            _outputDevices.Single(m => m.Id == id).Selected = true;

            // select the output device
            _deviceEnumerator.GetDevice(id).Selected = true;
        }

        private void SetDeviceVolume(string id, int value)
        {
            _doSendMessage = false;
            _deviceEnumerator.GetDevice(id).AudioEndpointVolume.MasterVolumeLevelScalar = value / 100F;
        }

        private void OnArduinoDeviceVolumeUpdate(ArduinoServiceDeviceVolumeUpdate obj)
        {
            _logger.LogDebug("{Class}.{Function}()", nameof(AudioService), nameof(OnArduinoDeviceVolumeUpdate));

            SetDeviceVolume(_outputDevices.Single(m => m.Selected).Id, obj.Volume);
        }

        private void OnMainWindowDeviceVolumeUpdate(MainWindowDeviceVolumeUpdate obj)
        {
            _logger.LogDebug("{Class}.{Function}()", nameof(AudioService), nameof(OnMainWindowDeviceVolumeUpdate));

            SetDeviceVolume(_outputDevices.Single(m => m.Selected).Id, obj.Volume);
        }

        public void Dispose()
        {
            _logger.LogDebug("{Class}.{Function}()", nameof(AudioService), nameof(Dispose));
            PubSub.Unsubscribe<AudioService, ArduinoServiceDeviceVolumeUpdate>(this);
            PubSub.Unsubscribe<AudioService, MainWindowDeviceVolumeUpdate>(this);
        }
    }
}