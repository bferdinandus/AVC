using MvvmCross.Plugin.Messenger;

namespace AVC.Core.Services
{
    public class VolumeUpdateMessage : MvxMessage
    {
        public int Volume { get; private set; }
        public bool DoUpdateDeviceVolume { get; set; }

        public VolumeUpdateMessage(object sender, int volume, bool doUpdateDeviceVolume) : base(sender)
        {
            Volume = volume;
            DoUpdateDeviceVolume = doUpdateDeviceVolume;
        }

    }
}