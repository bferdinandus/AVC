using System;
using System.IO.Ports;
using Microsoft.Extensions.Logging;

namespace AVC.Wpf.Services
{
    public interface ISerialCommunication
    {
    }

    public sealed class SerialCommunication : ISerialCommunication, IDisposable
    {
        private readonly ILogger<SerialCommunication> _logger;

        private readonly SerialPort _serialPort;

        public SerialCommunication(ILogger<SerialCommunication> logger)
        {
            _logger = logger;
            logger.LogInformation($"{nameof(SerialCommunication)}()");

            _serialPort = new SerialPort();
            _serialPort.PortName = "COM4";
            _serialPort.BaudRate = 115200;
            _serialPort.RtsEnable = true;
            _serialPort.DtrEnable = true;

            _serialPort.Open();




        }

        public void Dispose()
        {
            _logger.LogInformation($"{nameof(SerialCommunication)}.Dispose()");

            _serialPort.WriteLine("<0,Master,39>");

            _serialPort.Close();
            _serialPort.Dispose();
        }
    }
}