using Common;
using DataProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Agent.Models
{
    public class SensorsWatcher
    {
        private readonly object sync = new object();

        private readonly SensorDataProvider dataProvider;
        private readonly int queryIntervalMilliSec;
        private readonly int interrupIntervalMilliSec;
        private readonly AutoResetEvent shutdownRequest;

        // тут будут не только последние данные, а несколько предыдущих снапшотов. В PoC предлагаю брать только последние
        private Dictionary<string, double> lastCatchedData = new Dictionary<string, double>();

        public SensorsWatcher(SensorDataProvider dataProvider, int queryIntervalMilliSec, int interrupIntervalMilliSec = 1000)
        {
            this.dataProvider = dataProvider;
            this.queryIntervalMilliSec = queryIntervalMilliSec;
            this.interrupIntervalMilliSec = interrupIntervalMilliSec;

            shutdownRequest = new AutoResetEvent(false);

            var wathcerThread = new Thread(WatchProc)
            {
                IsBackground = false,
                Name = "WatcherThread",
            };
            wathcerThread.Start();
        }

        private void WatchProc()
        {
            while (true)
            {
                for (var i = 0; i < queryIntervalMilliSec / interrupIntervalMilliSec; ++i)
                    if (shutdownRequest.WaitOne(interrupIntervalMilliSec))
                        return; // тут кто-то сказал нам, что приложение нужно стопнуть

                lock (sync) // обновляем данные
                    lastCatchedData = dataProvider
                        .GetHardwareData()
                        .ToDictionary(k => k.SendorId, v => v.Value);
            }
        }

        public HardwareTree GetSensorsData()
        {
            lock (sync)
            {
                return new HardwareTree();
            }
        }
    }
}