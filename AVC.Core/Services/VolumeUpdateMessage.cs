using MvvmCross.Plugin.Messenger;

namespace AVC.Core.Services
{
    public class VolumeUpdateMessage : MvxMessage
    {
        public int Volume { get; private set; }

        public VolumeUpdateMessage(object sender, int volume) : base(sender)
        {
            Volume = volume;
        }

    }
}