using System;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PubSubNET;

namespace AVC.Wpf.Services
{
    public interface ISerialCommunication {}

    public sealed class SerialCommunication : ISerialCommunication, IDisposable
    {
        private readonly ILogger<SerialCommunication> _logger;

        private readonly SerialPort _serialPort;
        private readonly StringBuilder _incomingData = new();

        public SerialCommunication(ILogger<SerialCommunication> logger)
        {
            _logger = logger;
            _logger.LogDebug($"{nameof(SerialCommunication)}()");

            PubSub.Subscribe<SerialCommunication, ArduinoMessage>(this, OnArduinoMessageReceived);
            PubSub.Subscribe<SerialCommunication, AudioServiceDeviceVolumeUpdate>(this, OnDeviceVolumeUpdate);

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
            _logger.LogDebug($"{nameof(SerialCommunication)}.{nameof(DataReceivedHandler)}()");

            string inData = ((SerialPort) sender).ReadExisting();
            _logger.LogInformation("Data Received: {0}", inData);

            bool receiveInProgress = false;
            const char startMarker = '<';
            const char endMarker = '>';

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

        private void OnDeviceVolumeUpdate(AudioServiceDeviceVolumeUpdate obj)
        {
            _logger.LogDebug($"{nameof(SerialCommunication)}.{nameof(OnDeviceVolumeUpdate)}()");
            string arduinoInfo = $"<0,Master,{obj.Volume}>";
            _logger.LogInformation("Sending info to arduino: {0}", arduinoInfo);

            _serialPort.WriteLine(arduinoInfo);
        }

        private void OnArduinoMessageReceived(ArduinoMessage obj)
        {
            _logger.LogDebug($"{nameof(SerialCommunication)}.{nameof(OnArduinoMessageReceived)}()");
            _logger.LogInformation("Message: {0}", obj.Message);
            if (obj.Message == "Arduino is ready") {
                _serialPort.WriteLine("<0,Master,10>");
            }

            Regex arduinoCommandPattern = new Regex("(\\d),(\\w+?),(\\d+)");
            Match commandMatch = arduinoCommandPattern.Match(obj.Message);
            if (commandMatch.Success) {
                PubSub.Publish(new ArduinoDeviceVolumeUpdate(int.Parse(commandMatch.Groups[3].Value)));
            }
        }

        public void Dispose()
        {
            _logger.LogDebug($"{nameof(SerialCommunication)}.Dispose()");

            PubSub.Unsubscribe<SerialCommunication, ArduinoMessage>(this);
            PubSub.Unsubscribe<SerialCommunication, AudioServiceDeviceVolumeUpdate>(this);

            _serialPort.DataReceived -= DataReceivedHandler;
            _serialPort.Close();
            _serialPort.Dispose();
        }
    }
}