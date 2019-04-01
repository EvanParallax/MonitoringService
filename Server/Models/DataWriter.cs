using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web;

namespace Server.Entities
{
    public class DataWriter
    {
        private readonly object sync;

        private List<Agent> Agents;

        private HardwareContext Dbcontext;
        private HttpClient AgentClient;
        private readonly int queryIntervalMilliSec;
        private readonly int interrupIntervalMilliSec;
        private readonly AutoResetEvent shutdownRequest;

        // тут будут не только последние данные, а несколько предыдущих снапшотов. В PoC предлагаю брать только последние
        //private Dictionary<string, double> lastCatchedData = new Dictionary<string, double>();

        public DataWriter(object sync_obj, int queryIntervalMilliSec, int interrupIntervalMilliSec = 1000)
        {
            this.Agents = new List<Agent>();

            this.AgentClient = new HttpClient();

            this.sync = sync_obj;

            this.queryIntervalMilliSec = queryIntervalMilliSec;

            this.interrupIntervalMilliSec = interrupIntervalMilliSec;

            shutdownRequest = new AutoResetEvent(false);

            foreach (var agent in Dbcontext.Agents)
                Agents.Add(agent);

            var writerThread = new Thread(WriteProc)
            {
                IsBackground = false,
                Name = "WatcherThread",
            };
            writerThread.Start();
        }

        private void WriteProc()
        {
            while (true)
            {
                for (var i = 0; i < queryIntervalMilliSec / interrupIntervalMilliSec; ++i)
                    if (shutdownRequest.WaitOne(interrupIntervalMilliSec))
                        return; // тут кто-то сказал нам, что приложение нужно стопнуть

                lock (sync)
                { // обновляем данные
                    using (Dbcontext = new HardwareContext())
                    {
                        Write();
                    }
                }
            }
        }

        private void Write() {
            foreach (var agent in Agents)
            {
                var response = AgentClient.GetAsync(agent.Endpoint).Result;
                string responseBody = response.Content.ReadAsStringAsync().Result;
                Cover CurrAgentCover = JsonConvert.DeserializeObject<Cover>(responseBody);

                Session CurrAgentSession = new Session()
                {
                    AgentId = agent.Id,
                    Id = Guid.NewGuid(),
                    ServerTime = DateTime.Now,
                    AgentTime = CurrAgentCover.Header.AgentTime,
                    Error = CurrAgentCover.Header.ErrorMsg
                };

                var CurrAgentContainer = new Container()
                {
                    AgentId = agent.Id,
                    Id = Guid.NewGuid(),
                    ParentContainerId = Guid.NewGuid()
                };
                Dbcontext.Containers.Add(CurrAgentContainer);

                var sensors = CurrAgentCover.HardwareTree.Sensors.ToArray();
                foreach(var sensor in sensors)
                {
                    var curr_sensor = new Sensor()
                    {
                        Id = sensor.Id,
                        ContainerId = CurrAgentContainer.Id,
                        Type = sensor.Type
                    };
                    Dbcontext.Sensors.Add(curr_sensor);

                    var curr_metric = new Metric()
                    {
                        SessionId = CurrAgentSession.Id,
                        SensorId = Guid.NewGuid(),
                        Valued = sensor.Value
                    };
                    Dbcontext.Metrics.Add(curr_metric);
                }

                Dbcontext.SaveChanges();
            }
        }

       
    }
}