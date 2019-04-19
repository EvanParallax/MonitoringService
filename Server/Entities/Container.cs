using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Utils
{
    public class Container
    {
        public string DeviceName { get; set; }
        public Guid Id { get; set; }
        public Guid AgentId { get; set; }
        public Guid? ParentContainerId { get; set; }
    }
}