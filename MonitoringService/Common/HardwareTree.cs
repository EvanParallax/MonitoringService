using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class HardwareTree
    {
        public List<HardwareTree> Subhardware { get; set; }

        public List<SensorHW> Sensors { get; set; }

        public HardwareTree()
        {
            Subhardware = new List<HardwareTree>();
            Sensors = new List<SensorHW>();
        }
    }
}
