using AVC.Core.Models;
using AVC.Core.Services;
using Prism.Mvvm;

namespace AVC.UI.ViewModels
{
    public class ArduinoStatusViewModel : BindableBase
    {
        private ArduinoStatus _arduinoStatus;

        public ArduinoStatus ArduinoStatus {
            get => _arduinoStatus;
            set => SetProperty(ref _arduinoStatus, value);
        }

        private string[] _serialPorts;

        public string[] SerialPorts {
            get => _serialPorts;
            set => SetProperty(ref _serialPorts, value);
        }

        public ArduinoStatusViewModel(IArduinoService arduinoService)
        {
            ArduinoStatus = arduinoService.Status();
            SerialPorts = arduinoService.GetPorts();
        }
    }
}