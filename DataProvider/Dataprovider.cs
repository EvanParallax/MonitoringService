using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataProvider
{
    public class SensorDataProvider
    {
        private readonly Random rnd = new Random();

        public (string SendorId, double Value)[] GetHardwareData()
        {
            return Enumerable.Range(0, 10)
                .Select(i => (SendorId: Guid.NewGuid().ToString(), Value: rnd.NextDouble()))
                .ToArray();
        }
    }
}
