using Common;
using System.Collections.Generic;
using JetBrains.Annotations;
using System;
using System.Threading.Tasks;

namespace Agent.Utils
{
    public interface ISensorWatcher : IDisposable
    {
        [CanBeNull]
        HardwareTree GetSensorsData();

        Task WatchProc();

        new void Dispose();
    }

    class SensorWatcher : ISensorWatcher
    {
        private readonly IHardwareDataProvider provider;
        private readonly object sync = new object();
        private readonly Queue<HardwareTree> lastCatchedData;

        public SensorWatcher()
        {
            provider = new HardwareDataProvider();
            lastCatchedData = new Queue<HardwareTree>();
        }

        public async Task WatchProc()
        {
            await Task.Run(() => 
            { 
                var tree = provider.GetSystemInfo();

                lock (sync)
                    lastCatchedData.Enqueue(tree);
            });
        }

        public HardwareTree GetSensorsData()
        {
            lock (sync)
                return lastCatchedData.Count > 0 ? lastCatchedData.Dequeue() : null;
        }

        public void Dispose()
        {
            provider.Dispose();
        }
    }
}