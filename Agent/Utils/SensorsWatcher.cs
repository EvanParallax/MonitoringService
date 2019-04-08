using Common;
using System.Collections.Generic;
using JetBrains.Annotations;
using System;

namespace Agent.Utils
{
    public interface ISensorWatcher
    {
        [CanBeNull]
        HardwareTree GetSensorsData();

        void WatchProc();
    }

    class SensorWatcher : ISensorWatcher
    {
        private readonly object sync = new object();
        private readonly Random rand = new Random();
        private readonly Queue<HardwareTree> lastCatchedData;

        public SensorWatcher()
        {
            lastCatchedData = new Queue<HardwareTree>();
        }

        public void WatchProc()
        {
            var tree = new HardwareTree();
            tree.Sensors.Add(new SensorHW("1", "PCH", rand.Next(30, 50)));

            var cpuTree = new HardwareTree();
            cpuTree.Sensors.Add(new SensorHW("2", "cpu_temp", rand.Next(30, 80)));

            var gpuTree = new HardwareTree();
            gpuTree.Sensors.Add(new SensorHW("3", "gpu_temp", rand.Next(40, 88)));

            var hddTree = new HardwareTree();
            hddTree.Sensors.Add(new SensorHW("4", "hdd_temp", rand.Next(25, 35)));
            var hdddTree = new HardwareTree();
            hdddTree.Sensors.Add(new SensorHW("66", "hd2d_temp", rand.Next(25, 35)));

            tree.Subhardware.Add(cpuTree);
            tree.Subhardware.Add(gpuTree);
            tree.Subhardware.Add(hddTree);
            tree.Subhardware.Add(hdddTree);

            lock (sync) 
                lastCatchedData.Enqueue(tree);
        }

        public HardwareTree GetSensorsData()
        {
            lock (sync)
                return lastCatchedData.Count > 0 ? lastCatchedData.Dequeue() : null;
        }
    }
}