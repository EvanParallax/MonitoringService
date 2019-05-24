using Common;
using Server.DTOs;
using Server.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

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
        private List<(Envelope env, Guid id, int del)> envelopeList;

        public SessionWriter(IDataContext ctx, IDataReceiver rcvr, IDataProvider prvdr, IHierarchyWriter hWriter, IMetricWriter mWriter)
        {
            dbContext = ctx;
            receiver = rcvr;
            provider = prvdr;
            hierarchyWriter = hWriter;
            metricWriter = mWriter;
            locking = new object();
            envelopeList = new List<(Envelope env, Guid id, int del)>();
        }

        private void ParallelIteration(Agent agent)
        {

            if (!agent.IsEnabled)
                return;
            var requestTime = DateTime.Now;
            (Envelope env, int del) result;
            try
            {
                result = receiver.GetDataAsync(agent.Endpoint).Result;
            }
            catch (AggregateException ae)
            {
                result = (new Envelope()
                {
                    Header = new Header()
                    {
                        ErrorMsg = ae.InnerException.Message,
                        AgentTime = DateTime.Now
                    }
                },
                0);
            }

            Envelope currEnvelope = result.env;
            int del = result.del;
            currEnvelope.Header.RequestTime = requestTime;

            lock(locking)
                envelopeList.Add( (currEnvelope, agent.Id, del) ); 
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }

        public void WriteSession()
        {
            envelopeList.Clear();
            var options = new ParallelOptions()
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            Parallel.ForEach(dbContext.Agents, options, ParallelIteration);

                foreach (var item in envelopeList)
                {
                    Session currAgentSession = new Session()
                    {
                        AgentId = item.id,
                        Id = Guid.NewGuid(),
                        ServerTime = item.env.Header.RequestTime,
                        AgentTime = item.env.Header.AgentTime,
                        Error = item.env.Header.ErrorMsg
                    };

                    dbContext.Sessions.Add(currAgentSession);

                    if (item.env.HardwareTree != null)
                    {
                        var dbDelay = (item.del / 5) * 5;
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
                            AgentId = item.id,
                            DelayId = delay.Id,
                            Date = DateTime.Now
                        });

                        NewDataDTO data = provider.GetNewData(
                            item.env.HardwareTree,
                            item.id,
                            null);

                        hierarchyWriter.Write(data, item.id);

                        metricWriter.WriteMetrics(item.env.HardwareTree, currAgentSession.Id);
                    }
                }

                dbContext.SaveChanges();
        }
    }
}