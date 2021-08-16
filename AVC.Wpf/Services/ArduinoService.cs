using System;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using AVC.Wpf.PubSubMessages;
using Microsoft.Extensions.Logging;
using PubSubNET;

namespace AVC.Wpf.Services
{
    public interface IArduinoService {}

    public sealed class ArduinoService : IArduinoService, IDisposable
    {
        private readonly ILogger<ArduinoService> _logger;

        private readonly SerialPort _serialPort;
        private readonly StringBuilder _incomingData = new();

        public ArduinoService(ILogger<ArduinoService> logger)
        {
            _logger = logger;
            _logger.LogTrace($"{nameof(ArduinoService)}()");

            PubSub.Subscribe<ArduinoService, ArduinoMessage>(this, OnArduinoMessageReceived);
            PubSub.Subscribe<ArduinoService, AudioServiceDeviceVolumeUpdate>(this, OnAudioServiceDeviceVolumeUpdate);
            PubSub.Subscribe<ArduinoService, MainWindowDeviceVolumeUpdate>(this, OnMainWindowDeviceVolumeUpdate);

            _serialPort = new SerialPort();
            _serialPort.PortName = "COM4";
            _serialPort.BaudRate = 115200;
            _serialPort.RtsEnable = true;
            _serialPort.DtrEnable = true;

            _serialPort.DataReceived += DataReceivedHandler;

            _serialPort.Open();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            _logger.LogTrace($"{nameof(ArduinoService)}.{nameof(DataReceivedHandler)}()");

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
                        PubSub.Publish(new ArduinoMessage(_incomingData.ToString()));
                        _incomingData.Clear();
                    }
                } else if (rc == startMarker) {
                    receiveInProgress = true;
                }
            }
        }

        private void OnArduinoMessageReceived(ArduinoMessage obj)
        {
            _logger.LogDebug("{Class}.{Function}()", nameof(ArduinoService), nameof(OnArduinoMessageReceived));
            _logger.LogTrace("{Class} Message: {0}", nameof(ArduinoService), obj.Message);
            if (obj.Message == "Arduino is ready") {
                _serialPort.WriteLine("<0,Master,10>");
            }

            Regex arduinoCommandPattern = new Regex(@"(\d),([\w,\s]+),(\d+)");
            Match commandMatch = arduinoCommandPattern.Match(obj.Message);
            if (commandMatch.Success) {
                PubSub.Publish(new ArduinoServiceDeviceVolumeUpdate(int.Parse(commandMatch.Groups[3].Value)));
            }
        }

        private void OnAudioServiceDeviceVolumeUpdate(AudioServiceDeviceVolumeUpdate obj)
        {
            _logger.LogDebug("{Class}.{Function}()", nameof(ArduinoService), nameof(OnAudioServiceDeviceVolumeUpdate));

            string arduinoInfo = $"<0,{obj.Name.Substring(0, Math.Min(9, obj.Name.Length))},{obj.Volume}>";
            _logger.LogDebug("{Class} Sending info to arduino: {0}", nameof(ArduinoService), arduinoInfo);

            _serialPort.WriteLine(arduinoInfo);
        }

        private void OnMainWindowDeviceVolumeUpdate(MainWindowDeviceVolumeUpdate obj)
        {
            _logger.LogDebug("{Class}.{Function}()", nameof(ArduinoService), nameof(OnMainWindowDeviceVolumeUpdate));

            string arduinoInfo = $"<0,{obj.Name.Substring(0, Math.Min(9, obj.Name.Length))},{obj.Volume}>";
            _logger.LogDebug("{Class} Sending info to arduino: {0}", nameof(ArduinoService), arduinoInfo);

            _serialPort.WriteLine(arduinoInfo);
        }

        public void Dispose()
        {
            _logger.LogDebug("{Class}.{Function}()", nameof(ArduinoService), nameof(Dispose));

            PubSub.Unsubscribe<ArduinoService, ArduinoMessage>(this);
            PubSub.Unsubscribe<ArduinoService, AudioServiceDeviceVolumeUpdate>(this);
            PubSub.Unsubscribe<ArduinoService, MainWindowDeviceVolumeUpdate>(this);

            _serialPort.DataReceived -= DataReceivedHandler;
            _serialPort.Close();
            _serialPort.Dispose();
        }
    }
}