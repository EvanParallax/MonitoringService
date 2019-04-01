using Common;
using Server.Utils;
using System;
using System.Collections.Generic;

namespace Agent.Utils
{
    public interface ISensorWatcher
    {
        Envelope GetSensorsData();

        void WatchProc();
    }

    class SensorWatcher : ISensorWatcher
    {
        private object sync = new object();
        private Queue<HardwareTree> lastCatchedData;

        public SensorWatcher()
        {
            lastCatchedData = new Queue<HardwareTree>();
        }

        public void WatchProc()
        {
            var tree = new HardwareTree();
            tree.Sensors.Add(new SensorHW("1", "cpu_temp", 35.4f));
            tree.Sensors.Add(new SensorHW("2", "gpu_temp", 83.2f));
            tree.Sensors.Add(new SensorHW("3", "mb_temp", 32.4f));
            var anotherTree = new HardwareTree();
            anotherTree.Sensors.Add(new SensorHW("4", "winchester", 45.4f));
            tree.Subhardware.Add(anotherTree);
            lock (sync) 
                lastCatchedData.Enqueue(tree);
        }

        public Envelope GetSensorsData()
        {
            lock (sync)
            {
                if (lastCatchedData.Count != 0)
                    return new Envelope()
                    {
                        Header = new Header
                        {
                            AgentId = Guid.NewGuid(),
                            AgentTime = DateTime.Now,
                            ErrorMsg = ""
                        },
                        HardwareTree = lastCatchedData.Dequeue()
                    };
            }
            return new Envelope()
            {
                Header = new Header
                {
                    AgentId = Guid.NewGuid(),
                    AgentTime = DateTime.Now,
                    ErrorMsg = "Queue is empty"
                },
                HardwareTree = null
            };
        }
    }
}