using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Server.Utils
{
    public class DataWriter: IDisposable
    {
        private List<Agent> Agents;

        private IDataReciever reciever;

        private IDataContext DbContext;

        private bool disposed = false;

        // тут будут не только последние данные, а несколько предыдущих снапшотов. В PoC предлагаю брать только последние
        //private Dictionary<string, double> lastCatchedData = new Dictionary<string, double>();

        public DataWriter(IDataContext ctx, IDataReciever rcvr)
        {
            reciever = rcvr;
            DbContext = ctx;
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Освобождаем управляемые ресурсы
                }
                disposed = true;
            }
        }

        ~DataWriter()
        {
            Dispose(false);
        }

        private void WriteSensorsData(HardwareTree tree, Agent agent, Session session, Guid containerParentId)
        {
            if (tree == null)
                return;


            var CurrContainer = new Container()
            {
                AgentId = agent.Id,
                Id = Guid.NewGuid(),
                ParentContainerId = containerParentId
            };

            foreach (var sensor in tree.Sensors)
            {
                Agents = DbContext.Agents.ToList();
                foreach (var container in DbContext.Containers)
                    DbContext.Containers.Remove(container);

                foreach (var snsr in DbContext.Sensors)
                    DbContext.Sensors.Remove(snsr);

                var curr_sensor = new Sensor()
                {
                    Id = sensor.Id,
                    ContainerId = CurrContainer.Id,
                    Type = sensor.Type
                };
                DbContext.Sensors.Add(curr_sensor);

                var curr_metric = new Metric()
                {
                    SessionId = session.Id,
                    SensorId = Guid.NewGuid(),
                    Valued = sensor.Value
                };
                DbContext.Metrics.Add(curr_metric);
            }

            if (tree.Subhardware.Count != 0)
                foreach (var sh in tree.Subhardware)
                    WriteSensorsData(sh, agent, session, CurrContainer.Id);
        }

        private Session WriteSessiaon(Guid agentId, Header currEnvelopeHeader)
        {
            Session CurrAgentSession = new Session()
            {
                AgentId = agentId,
                Id = Guid.NewGuid(),
                ServerTime = DateTime.Now,
                AgentTime = currEnvelopeHeader.AgentTime,
                Error = currEnvelopeHeader.ErrorMsg
            };
            DbContext.Sessions.Add(CurrAgentSession);

            return CurrAgentSession;
        }

        public void WriteProc()
        {
            var agent = new Agent()
            {
                Id = Guid.NewGuid(),
                CredId = Guid.NewGuid(),
                Endpoint = "http://localhost:51221/api/Values",
                AgentVersion = ""

            };
            //foreach (var agent in Agents)
            //{
                Envelope CurrAgentEnvelope = reciever.GetData(agent.Endpoint);

                var session = WriteSessiaon(agent.Id, CurrAgentEnvelope.Header);

                WriteSensorsData(new HardwareTree(), agent, session, Guid.Empty);

                DbContext.SaveChanges();
           // }

        }
    }
}