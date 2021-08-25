using System;

namespace AVC.Core.Events
{
    public class UIDeviceUpdateMessage
    {
        public int Volume { get; init; }
        public Guid Id { get; init; }
    }
}