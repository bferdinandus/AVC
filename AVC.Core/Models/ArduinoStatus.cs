using Prism.Mvvm;

namespace AVC.Core.Models
{
    public class ArduinoStatus : BindableBase
    {
        private string _serialPort;
        private bool _serialPortOpen;
        private string _lastErrorMessage;
        private bool _arduinoReady;
        private string _lastMessageReceived;
        private string _lastMessageSent;

        public string SerialPort {
            get => _serialPort;
            set => SetProperty(ref _serialPort, value);
        }

        public bool SerialPortOpen {
            get => _serialPortOpen;
            set => SetProperty(ref _serialPortOpen, value);
        }

        public string LastErrorMessage {
            get => _lastErrorMessage;
            set => SetProperty(ref _lastErrorMessage, value);
        }

        public bool ArduinoReady {
            get => _arduinoReady;
            set => SetProperty(ref _arduinoReady, value);
        }

        public string LastMessageReceived {
            get => _lastMessageReceived;
            set => SetProperty(ref _lastMessageReceived, value);
        }

        public string LastMessageSent {
            get => _lastMessageSent;
            set => SetProperty(ref _lastMessageSent, value);
        }
    }
}