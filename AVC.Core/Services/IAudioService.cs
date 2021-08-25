using System;
using System.Collections.Generic;
using AVC.Core.Models;

namespace AVC.Core.Services
{
    public interface IAudioService
    {
        List<AudioDeviceModel> GetActiveOutputDevices();
        void SelectDeviceById(Guid id);
    }
}