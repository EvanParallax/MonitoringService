using Common;
using Server.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.DTOs
{
    public class ContainerDTO
    {
        public Guid ContainerId { get; set; }
        public Guid? ParentId { get; set; }
        public List<SensorHW> Sensors { get; set; }
    }
}