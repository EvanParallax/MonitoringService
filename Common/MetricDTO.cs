using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class MetricDTO
    {
        public DateTime Session { get; set; } // replace with SessionId?
        public string Stype { get; set; }
        public float Svalue { get; set; }
    }
}
