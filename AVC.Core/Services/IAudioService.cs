using System;
using System.Collections.ObjectModel;
using AVC.Core.Models;

namespace AVC.Core.Services
{
    public interface IAudioService
    {
        ObservableCollection<AudioDeviceModel> GetActiveOutputDevices();
        void SelectDeviceById(Guid id);
    }
}