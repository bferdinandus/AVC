using AVC.Core.Models;
using AVC.Core.Services;
using Prism.Mvvm;

namespace AVC.UI.ViewModels
{
    public class ArduinoStatusViewModel : BindableBase
    {
        private ArduinoStatus _arduinoStatus;
        private string[] _serialPorts;
        private string _selectedSerialPort;

        public ArduinoStatus ArduinoStatus {
            get => _arduinoStatus;
            set => SetProperty(ref _arduinoStatus, value);
        }

        public string[] SerialPorts {
            get => _serialPorts;
            set => SetProperty(ref _serialPorts, value);
        }

        public string SelectedSerialPort {
            get => _selectedSerialPort;
            set => SetProperty(ref _selectedSerialPort, value);
        }

        public ArduinoStatusViewModel(IArduinoService arduinoService)
        {
            ArduinoStatus = arduinoService.Status();
            SerialPorts = arduinoService.GetPorts();
        }
    }
}