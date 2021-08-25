using Prism.Mvvm;

namespace AVC.Core.Models
{
    public class ArduinoStatus : BindableBase
    {
        private bool _serialPortOpen;
        private bool _arduinoReady;
        private string _lastMessage;
        private string _lastErrorMessage;

        public bool SerialPortOpen {
            get => _serialPortOpen;
            set => SetProperty(ref _serialPortOpen, value);
        }

        public bool ArduinoReady {
            get => _arduinoReady;
            set => SetProperty(ref _arduinoReady, value);
        }

        public string LastMessage {
            get => _lastMessage;
            set => SetProperty(ref _lastMessage, value);
        }

        public string LastErrorMessage {
            get => _lastErrorMessage;
            set => SetProperty(ref _lastErrorMessage, value);
        }
    }
}