namespace AVC.Wpf.Services
{
    public class ArduinoMessage
    {
        public string Message { get; }

        public ArduinoMessage(string message)
        {
            Message = message;
        }
    }
}