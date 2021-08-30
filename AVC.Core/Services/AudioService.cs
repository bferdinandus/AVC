using System;
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
    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public class AudioService : IAudioService, IDisposable
    {
        // services
        private readonly IAudioController _audioController;
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger<AudioService> _logger;

        //private variables
        private readonly IList<AudioDeviceModel> _outputDevices = new List<AudioDeviceModel>();
        private readonly IList<AudioSessionModel> _audioSessions = new List<AudioSessionModel>();
        private readonly IDisposable _deviceChangedSubscription;

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

            _eventAggregator.GetEvent<UiDeviceUpdateEvent>().Subscribe(OnUIDeviceUpdateEvent);
            _eventAggregator.GetEvent<ArduinoDeviceUpdateEvent>().Subscribe(OnArduinoDeviceUpdateEvent);

            //_deviceChangedSubscription = _audioController.AudioDeviceChanged.Subscribe(OnAudioDeviceChangedEvent);

            RefreshActiveOutputDevices();
        }

        public IEnumerable<AudioDeviceModel> GetActiveOutputDevices(bool refresh)
        {
            if (refresh) {
                RefreshActiveOutputDevices();
            }

            return _outputDevices;
        }

        public void SelectDeviceById(Guid id)
        {
            if (_outputDevices.Single(d => d.Id == id).Selected) {
                // given Id already selected => gtfo
                return;
            }

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

        public void NextDevice()
        {
            int index = _outputDevices.IndexOf(_outputDevices.Single(d => d.Selected));

            index++;
            if (index >= _outputDevices.Count) {
                index = 0;
            }

            SelectDeviceById(_outputDevices[index].Id);
        }

        /*
         * Events functions
         */
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

        private void OnUIDeviceUpdateEvent(UiDeviceUpdateMessage message)
        {
            SetDeviceVolume(message.Id, message.Volume);
        }

        private void OnAudioDeviceChangedEvent(DeviceChangedArgs args)
        {
            _logger.LogDebug("{0} - {1}", args.Device.Name, args.ChangedType.ToString());
            _logger.LogDebug("State {s}", args.Device.State);

            if (args.Device.DeviceType == DeviceType.Playback && args.ChangedType == DeviceChangedType.StateChanged) {
                if (args.Device.State == DeviceState.Active) {
                    AddDeviceModelToList(args.Device);
                }

                if (args.Device.State != DeviceState.Active) {
                    RemoveDeviceModelFromList(args.Device.Id);
                }

                _eventAggregator.GetEvent<DeviceChangedEvent>().Publish();
            }
        }

        private void OnVolumeChangedEvent(DeviceVolumeChangedArgs args)
        {
            // update the model and send eventMessage
            AudioDeviceModel deviceModel = _outputDevices.Single(d => d.Id == args.Device.Id);

            _logger.LogDebug("device volume: {deviceVolume} new volume: {volume}", deviceModel.Volume, (int) args.Device.Volume);
            if (deviceModel.Volume == (int) args.Device.Volume) {
                return;
            }

            deviceModel.Volume = (int) args.Device.Volume;

            _eventAggregator.GetEvent<DeviceUpdateEvent>().Publish(new DeviceUpdateMessage { DeviceName = args.Device.Name, Volume = deviceModel.Volume });
        }

        /*
         * Private functions
         */
        private void RefreshActiveOutputDevices()
        {
            while (_outputDevices.Count > 0) {
                RemoveDeviceModelFromList(_outputDevices.Last().Id);
            }

            IEnumerable<IDevice> devices = _audioController.GetDevices(DeviceType.Playback, DeviceState.Active);

            foreach (IDevice device in devices) {
                AddDeviceModelToList(device);
            }
        }

        private void AddDeviceModelToList(IDevice device)
        {
            AudioDeviceModel deviceModel = new() {
                Id = device.Id,
                FullName = device.Name,
                Selected = device.IsDefaultDevice,
                Volume = (int) device.Volume,
                Muted = device.IsMuted,
                IconPath = device.IconPath,
                VolumeChangedSubscription = device.VolumeChanged.Subscribe(OnVolumeChangedEvent)
            };

            _outputDevices.Add(deviceModel);
        }

        private void RemoveDeviceModelFromList(Guid id)
        {
            AudioDeviceModel deviceModel = _outputDevices.Single(d => d.Id == id);
            deviceModel.Dispose();

            _outputDevices.Remove(deviceModel);
        }

        private void SetDeviceVolume(Guid id, int value)
        {
            _audioController.GetDevice(id).SetVolumeAsync(value);
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

        private void ReleaseUnmanagedResources()
        {
            // release unmanaged resources here
            while (_outputDevices.Count > 0) {
                // RemoveDeviceModelFromList Disposes of the subscription.(and clears the list.
                RemoveDeviceModelFromList(_outputDevices.Last().Id);
            }
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing) {
                _audioController?.Dispose();
                _deviceChangedSubscription?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AudioService()
        {
            Dispose(false);
        }
    }
}