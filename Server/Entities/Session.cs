using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Utils
{
    public class Session
    {
        public Guid Id { get; set; }


        public Guid AgentId { get; set; }
        public DateTime AgentTime { get; set; }
        public DateTime ServerTime { get; set; }
        public string Error { get; set; }
    }
}