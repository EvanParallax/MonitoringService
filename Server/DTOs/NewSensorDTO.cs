using Common;
using System;

namespace Server.DTOs
{
    public class NewSensorDTO
    {
        public SensorHW Sensor { get; set; }
        public Guid ContainerId { get; set; }
    }
}