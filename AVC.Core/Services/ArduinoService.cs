using System;
using System.IO.Ports;
using System.Text;
using AVC.Core.Events;
using Microsoft.Extensions.Logging;
using Prism.Events;

namespace AVC.Core.Services
{
    public interface IArduinoService {}

    public sealed class ArduinoService : IArduinoService, IDisposable
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger<ArduinoService> _logger;

        private readonly SerialPort _serialPort;
        private readonly StringBuilder _incomingData = new();

        private bool _arduinoReady = false;

        public ArduinoService(IEventAggregator eventAggregator,
                              ILogger<ArduinoService> logger)
        {
            _eventAggregator = eventAggregator;
            _logger = logger;

            _eventAggregator.GetEvent<ArduinoMessageEvent>().Subscribe(OnArduinoMessage);
            _eventAggregator.GetEvent<DeviceUpdateEvent>().Subscribe(OnDeviceUpdate);

            _serialPort = new SerialPort();
            _serialPort.PortName = "COM4";
            _serialPort.BaudRate = 115200;
            _serialPort.DtrEnable = true;

            _serialPort.DataReceived += DataReceivedHandler;

            _serialPort.Open();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            bool receiveInProgress = false;
            const char startMarker = '<';
            const char endMarker = '>';

            string inData = ((SerialPort) sender).ReadExisting();
            foreach (char rc in inData) {
                if (receiveInProgress) {
                    if (rc != endMarker) {
                        _incomingData.Append(rc);
                    } else {
                        receiveInProgress = false;
                        _eventAggregator.GetEvent<ArduinoMessageEvent>().Publish(_incomingData.ToString());
                        _incomingData.Clear();
                    }
                } else if (rc == startMarker) {
                    receiveInProgress = true;
                }
            }
        }

        private void OnArduinoMessage(string message)
        {
            string[] messageParts = message.Split(':');

            bool commandParseSuccess = Enum.TryParse(messageParts[1], true, out ArduinoCommands arduinoCommand);

            if (!commandParseSuccess) {
                return;
            }

            switch (arduinoCommand) {
                case ArduinoCommands.Ready:
                    // 0:cmd 1:ready
                    _arduinoReady = true;
                    _serialPort.WriteLine("<0,Master,10>");
                    break;
                case ArduinoCommands.Vol:
                    // 0:cmd 1:vol 2:(index) 3:(volume)
                    _eventAggregator.GetEvent<ArduinoDeviceUpdateEvent>()
                                    .Publish(new ArduinoDeviceUpdateMessage { Channel = int.Parse(messageParts[2]), Volume = int.Parse(messageParts[3]) });
                    break;
                case ArduinoCommands.Switch:
                    // 0:cmd 1:switch
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(arduinoCommand));
            }
        }

        private void OnDeviceUpdate(DeviceUpdateMessage message)
        {
            // device is on channel 0 of the arduino
            string arduinoInfo = $"<0,{message.DeviceName[..Math.Min(9, message.DeviceName.Length)].Trim()},{message.Volume}>";
            _serialPort.WriteLine(arduinoInfo);
        }

        // private void OnArduinoMessageReceived(ArduinoMessage obj)
        // {
        //     _logger.LogDebug("{Class}.{Function}()", nameof(ArduinoService), nameof(OnArduinoMessageReceived));
        //     _logger.LogTrace("{Class} Message: {0}", nameof(ArduinoService), obj.Message);
        //     if (obj.Message == "Arduino is ready") {
        //         _serialPort.WriteLine("<0,Master,10>");
        //     }
        //
        //     Regex arduinoCommandPattern = new Regex(@"(\d),([\w,\s]+),(\d+)");
        //     Match commandMatch = arduinoCommandPattern.Match(obj.Message);
        //     if (commandMatch.Success) {
        //         PubSub.Publish(new ArduinoServiceDeviceVolumeUpdate(int.Parse(commandMatch.Groups[3].Value)));
        //     }
        // }
        //
        // private void OnAudioServiceDeviceVolumeUpdate(AudioServiceDeviceVolumeUpdate obj)
        // {
        //     _logger.LogDebug("{Class}.{Function}()", nameof(ArduinoService), nameof(OnAudioServiceDeviceVolumeUpdate));
        //
        //     string arduinoInfo = $"<0,{obj.Name.Substring(0, Math.Min(9, obj.Name.Length))},{obj.Volume}>";
        //     _logger.LogDebug("{Class} Sending info to arduino: {0}", nameof(ArduinoService), arduinoInfo);
        //
        //     _serialPort.WriteLine(arduinoInfo);
        // }
        //
        // private void OnMainWindowDeviceVolumeUpdate(MainWindowDeviceVolumeUpdate obj)
        // {
        //     _logger.LogDebug("{Class}.{Function}()", nameof(ArduinoService), nameof(OnMainWindowDeviceVolumeUpdate));
        //
        //     string arduinoInfo = $"<0,{obj.Name.Substring(0, Math.Min(9, obj.Name.Length))},{obj.Volume}>";
        //     _logger.LogDebug("{Class} Sending info to arduino: {0}", nameof(ArduinoService), arduinoInfo);
        //
        //     _serialPort.WriteLine(arduinoInfo);
        // }

        public void Dispose()
        {
            _logger.LogTrace("{Function}()", nameof(Dispose));

            // PubSub.Unsubscribe<ArduinoService, ArduinoMessage>(this);
            // PubSub.Unsubscribe<ArduinoService, AudioServiceDeviceVolumeUpdate>(this);
            // PubSub.Unsubscribe<ArduinoService, MainWindowDeviceVolumeUpdate>(this);

            _serialPort.DataReceived -= DataReceivedHandler;
            _serialPort.Close();
            _serialPort.Dispose();
        }
    }
}