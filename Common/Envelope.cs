﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Header
    {
        public Guid AgentId { get; set; }
        public DateTime AgentTime { get; set; }
        public DateTime RequestTime { get; set; }
        public string ErrorMsg { get; set; }
    }
    public class Envelope
    {
        public Header Header { get; set; }
        public HardwareTree HardwareTree { get; set; }
    }
}
