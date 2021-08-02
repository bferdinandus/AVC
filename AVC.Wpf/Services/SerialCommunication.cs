using System;
using System.IO.Ports;
using Microsoft.Extensions.Logging;

namespace AVC.Core.Services
{
    public interface ISerialCommunication
    {
    }

    public class SerialCommunication : ISerialCommunication, IDisposable
    {
        private readonly ILogger<SerialCommunication> _logger;

        private readonly SerialPort _serialPort;

        public SerialCommunication(ILogger<SerialCommunication> logger)
        {
            _logger = logger;

            //_serialPort = new SerialPort();

            _logger.LogInformation("SerialCommunication initialized.");
        }

        public virtual void Dispose()
        {
            //_serialPort.Dispose();
            _logger.LogInformation("SerialCommunication Disposed.");
        }
    }
}