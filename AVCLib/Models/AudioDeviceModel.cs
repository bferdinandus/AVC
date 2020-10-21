namespace AVCLib.Models
{
    public class AudioDeviceModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public bool Active { get; set; }
        public bool Mute { get; set; }
        public int Volume { get; set; }

    }
}