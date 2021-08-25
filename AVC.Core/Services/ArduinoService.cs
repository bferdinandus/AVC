using System;
using System.IO.Ports;
using System.Text;
using AVC.Core.Events;
using AVC.Core.Models;
using Microsoft.Extensions.Logging;
using Prism.Events;

namespace AVC.Core.Services
{
    public sealed class ArduinoService : IArduinoService, IDisposable
    {
        // services
        private readonly IEventAggregator _eventAggregator;
        private readonly ILogger<ArduinoService> _logger;

        // private variables
        private readonly StringBuilder _incomingData = new();
        private readonly SerialPort _serialPort = new();
        private readonly ArduinoStatus _arduinoStatus = new();
        private long _lastDeviceUpdateEventSent = DateTime.Now.Ticks;
        private bool _arduinoReady;

        /*
         * constructor
         */
        public ArduinoService(IEventAggregator eventAggregator,
                              ILogger<ArduinoService> logger)
        {
            _eventAggregator = eventAggregator;
            _logger = logger;

            _eventAggregator.GetEvent<ArduinoMessageEvent>().Subscribe(OnArduinoMessageEvent);
            _eventAggregator.GetEvent<DeviceUpdateEvent>().Subscribe(OnDeviceUpdateEvent);

            ConnectArduino();
        }

        public ArduinoStatus Status() => _arduinoStatus;

        public string[] GetPorts() => SerialPort.GetPortNames();

        private void ConnectArduino()
        {
            try {
                _serialPort.PortName = "COM4";
                _serialPort.BaudRate = 115200;
                _serialPort.DtrEnable = true;

                _serialPort.DataReceived += DataReceivedHandler;

                _serialPort.Open();
                _arduinoStatus.SerialPortOpen = _serialPort.IsOpen;
            } catch (Exception e) {
                _logger.LogError(e, "Serial port not open");
                _arduinoStatus.SerialPortOpen = _serialPort.IsOpen;
                _arduinoStatus.LastErrorMessage = e.Message;
            }
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

        private void OnArduinoMessageEvent(string message)
        {
            _arduinoStatus.LastMessage = message;

            string[] messageParts = message.Split(':');

            bool commandParseSuccess = Enum.TryParse(messageParts[1], true, out ArduinoCommands arduinoCommand);

            if (!commandParseSuccess) {
                return;
            }

            switch (arduinoCommand) {
                case ArduinoCommands.Ready:
                    // 0:cmd 1:ready
                    _arduinoReady = true;
                    _arduinoStatus.ArduinoReady = true;
                    _serialPort.WriteLine("<0,Master,10>");
                    break;
                case ArduinoCommands.Vol:
                    // 0:cmd 1:vol 2:(index) 3:(volume)
                    _lastDeviceUpdateEventSent = DateTime.Now.Ticks;
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

        private void OnDeviceUpdateEvent(DeviceUpdateMessage message)
        {
            if (!_arduinoReady) {
                return;
            }

            if ((DateTime.Now.Ticks - _lastDeviceUpdateEventSent) / TimeSpan.TicksPerMillisecond < 50) {
                // block incoming device updated x milliseconds after the last ArduinoDeviceUpdateEvent publish
                _logger.LogDebug("Blocked {0}", nameof(DeviceUpdateEvent));

                return;
            }

            // device is on channel 0 of the arduino
            string arduinoInfo = $"<0,{message.DeviceName[..Math.Min(9, message.DeviceName.Length)].Trim()},{message.Volume}>";
            _logger.LogDebug("message:{message}", arduinoInfo);
            _serialPort.WriteLine(arduinoInfo);
        }

        public void Dispose()
        {
            _logger.LogTrace("{Function}()", nameof(Dispose));

            _serialPort.DataReceived -= DataReceivedHandler;
            _serialPort.Close();
            _serialPort.Dispose();
        }
    }
}