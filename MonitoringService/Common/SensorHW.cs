using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SensorHW : ISensorHW
    {
        public string Id { get ; set ; }
        public string Type { get; set; }
        public float Value { get ; set ; }

        public SensorHW(string id, string type, float value)
        {
            Id = id;
            Type = type;
            Value = value;
        }
    }
}
