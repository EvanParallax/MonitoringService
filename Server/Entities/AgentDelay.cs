using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Entities
{
    public class AgentDelay
    {
        public Guid AgentId { get; set; }
        public Guid DelayId { get; set; }
        public DateTime Date { get; set; }
    }
}