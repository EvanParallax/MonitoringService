using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Utils
{
    public class Sensor
    {
        public string Id { get; set; }
        public Guid ContainerId { get; set; }
        public string Type { get; set; }  
    }
}