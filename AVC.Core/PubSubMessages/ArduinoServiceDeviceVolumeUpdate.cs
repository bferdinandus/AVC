namespace AVC.Core.PubSubMessages
{
    public class ArduinoServiceDeviceVolumeUpdate : BaseVolumeUpdate
    {
        public ArduinoServiceDeviceVolumeUpdate(int volume) : base(volume) {}
    }
}