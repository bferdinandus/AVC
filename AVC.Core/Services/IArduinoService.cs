using AVC.Core.Models;

namespace AVC.Core.Services
{
    public interface IArduinoService
    {
        public ArduinoStatus Status();
        public string[] GetPorts();
    }
}