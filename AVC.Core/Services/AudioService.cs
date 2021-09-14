using System;
using System.Collections.Generic;
using System.Linq;
using AVC.Core.Events;
using AVC.Core.Models;
using CoreAudio;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Prism.Events;

namespace AVC.Core.Services
{
    [UsedImplicitly(ImplicitUseKindFlags.Access)]
    public class AudioService : IAudioService, IDisposable
    {
        // services
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger<AudioService> _logger;

        //private variables
        private readonly IList<AudioDeviceModel> _outputDevices = new List<AudioDeviceModel>();
        private readonly IList<AudioSessionModel> _audioSessions = new List<AudioSessionModel>();
        private readonly IDisposable _deviceChangedSubscription;

        /*
         * constructor
         */
        public AudioService(IEventAggregator eventAggregator,
                            ILogger<AudioService> logger)
        {
            _logger = logger;
            _eventAggregator = eventAggregator;

            // _eventAggregator.GetEvent<UiDeviceUpdateEvent>().Subscribe(OnUIDeviceUpdateEvent);
            // _eventAggregator.GetEvent<ArduinoDeviceUpdateEvent>().Subscribe(OnArduinoDeviceUpdateEvent);

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

        public void SelectDeviceById(string id)
        {
            /*if (_outputDevices.Single(d => d.Id == id).Selected) {
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
            device.SetAsDefault();*/
        }

        public void NextDevice()
        {
            /*int index = _outputDevices.IndexOf(_outputDevices.Single(d => d.Selected));

            index++;
            if (index >= _outputDevices.Count) {
                index = 0;
            }

            SelectDeviceById(_outputDevices[index].Id);*/
        }

        /*
         * Events functions
         */
        /*
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
        */

        /*private void OnUIDeviceUpdateEvent(UiDeviceUpdateMessage message)
        {
            SetDeviceVolume(message.Id, message.Volume);
        }*/

        /*
        private void OnAudioDeviceChangedEvent(DeviceChangedArgs args)
        {
            _logger.LogDebug("{0} - {1}  DeviceType:{t} State:{s} IsDefaultDevice:{f}",
                             args.Device.Name,
                             args.ChangedType,
                             args.Device.DeviceType,
                             args.Device.State,
                             args.Device.IsDefaultDevice);

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
        */

        /*
        private void OnVolumeChangedEvent(AudioVolumeNotificationData data)
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
        */

        /*
         * Private functions
         */
        private void RefreshActiveOutputDevices()
        {
            while (_outputDevices.Count > 0) {
                RemoveDeviceModelFromList(_outputDevices.Last().Id);
            }

            MMDeviceEnumerator enumerator = new MMDeviceEnumerator();

            foreach (MMDevice device in enumerator.EnumerateAudioEndPoints(EDataFlow.eRender, DEVICE_STATE.DEVICE_STATE_ACTIVE)) {
                AudioDeviceModel deviceModel = new AudioDeviceModel(device);
                deviceModel.PropertyChanged += (sender, args) => {
                    _logger.LogInformation("PropChange {0}", args.PropertyName);
                    _eventAggregator.GetEvent<DeviceUpdateEvent>().Publish(new DeviceUpdateMessage { DeviceName = (sender as AudioDeviceModel)?.FullName, Volume = deviceModel.Volume });
                };

                _outputDevices.Add(deviceModel);
            }

            MMDevice selectedDevice = enumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            _outputDevices.Single(o => o.Id == selectedDevice.ID).Selected = true;
        }

        private void RemoveDeviceModelFromList(string id)
        {
            AudioDeviceModel deviceModel = _outputDevices.Single(d => d.Id == id);
            deviceModel.Dispose();

            _outputDevices.Remove(deviceModel);
        }

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