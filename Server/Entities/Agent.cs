using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Utils
{
    public class Agent
    {
        public Guid Id { get; set; }
        public Guid CredId { get; set; }
        public string Endpoint { get; set; }
        public string OsType { get; set; }
        public string AgentVersion { get; set; }
    }
}