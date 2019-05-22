using Common;
using Server.DTOs;
using Server.Entities;
using System;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;

namespace Server.Utils
{
    public class SessionWriter : IDisposable
    {
        private readonly IDataContext dbContext;
        private readonly IDataReceiver receiver;
        private readonly IDataProvider provider;
        private readonly IHierarchyWriter hierarchyWriter;
        private readonly IMetricWriter metricWriter;
        private readonly object locking;

        public SessionWriter(IDataContext ctx, IDataReceiver rcvr, IDataProvider prvdr, IHierarchyWriter hWriter, IMetricWriter mWriter)
        {
            dbContext = ctx;
            receiver = rcvr;
            provider = prvdr;
            hierarchyWriter = hWriter;
            metricWriter = mWriter;
            locking = new object();
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }

        public async void WriteSession()
        {
            foreach (var agent in dbContext.Agents)
            {
                if(!agent.IsEnabled)
                    continue;

                var requestTime = DateTime.Now;
                var result = await receiver.GetDataAsync(agent.Endpoint);
                Envelope currEnvelope = result.env;
                int del = result.del;
                currEnvelope.Header.RequestTime = requestTime;

                Session currAgentSession = new Session()
                {
                    AgentId = agent.Id,
                    Id = Guid.NewGuid(),
                    ServerTime = currEnvelope.Header.RequestTime,
                    AgentTime = currEnvelope.Header.AgentTime,
                    Error = currEnvelope.Header.ErrorMsg
                };

                dbContext.Sessions.Add(currAgentSession);

                if(currEnvelope.HardwareTree != null)
                {
                    var dbDelay = (del / 5) * 5;
                    var delay = dbContext.Delays.FirstOrDefault(d => d.Value == dbDelay);
                    if (delay == null)
                    {
                        delay = new Delay();
                        delay.Value = dbDelay;
                        delay.Id = Guid.NewGuid();
                        dbContext.Delays.Add(delay);
                    }

                    dbContext.AgentDelays.Add(new AgentDelay()
                    {
                        AgentId = agent.Id,
                        DelayId = delay.Id,
                        Date = DateTime.Now
                    });

                    NewDataDTO data = provider.GetNewData(
                        currEnvelope.HardwareTree, 
                        agent.Id,
                        null);

                    hierarchyWriter.Write(data, agent);

                    metricWriter.WriteMetrics(currEnvelope.HardwareTree, currAgentSession.Id);
                }
            }
            lock(locking)
                dbContext.SaveChanges();
        }
    }
}