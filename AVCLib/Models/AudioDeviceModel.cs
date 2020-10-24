namespace AVCLib.Models
{
    public class AudioDeviceModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public bool Selected { get; set; }
        public bool Mute { get; set; }
        public float Volume { get; set; }

    }
}