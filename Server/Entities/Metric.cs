﻿using System;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Utils
{
    public class Metric
    {
        public Guid SessionId { get; set; }
        public string SensorId { get; set; }
        public float Value { get; set; } 
    }
}