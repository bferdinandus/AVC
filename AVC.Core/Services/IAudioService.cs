using System.Collections.Generic;
using AVC.Core.Models;

namespace AVC.Core.Services
{
    public interface IAudioService
    {
        IEnumerable<AudioDeviceModel> GetActiveOutputDevices(bool refresh);
        void SelectDeviceById(string id);
        public void NextDevice();
    }
}