namespace AVC.Core.PubSubMessages
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