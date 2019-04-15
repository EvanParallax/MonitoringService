using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Utils
{
    public class Metric
    {
        public Guid SessionId { get; set; }
        public Guid SensorId { get; set; }
        public float Valued { get; set; } 
    }
}