using System;

namespace AVC.Core.Events
{
    public class UiDeviceUpdateMessage
    {
        public int Volume { get; init; }
        public Guid Id { get; init; }
    }
}