using Common;
using Server.DTOs;
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

        public SessionWriter(IDataContext ctx, IDataReceiver rcvr, IDataProvider prvdr, IHierarchyWriter hWriter, IMetricWriter mWriter)
        {
            dbContext = ctx;
            receiver = rcvr;
            provider = prvdr;
            hierarchyWriter = hWriter;
            metricWriter = mWriter;
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }

        public void WriteSession()
        {
            foreach (var agent in dbContext.Agents)
            {
                if(!agent.IsEnabled)
                    continue;

                var requestTime = DateTime.Now;
                Envelope currEnvelope = receiver.GetDataAsync(agent.Endpoint).Result;
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
                    var delay = currEnvelope.
                        Header.RequestTime.
                        Subtract(currEnvelope.Header.AgentTime).
                        Milliseconds;

                    if (delay <= 100)
                    {
                        var answerTime = dbContext.AnswerTimes.Where(at => at.Delay == 100).FirstOrDefault();
                        if (answerTime == null)
                        {
                            answerTime = new AnswerTime();
                            answerTime.Delay = 100;
                            answerTime.Id = Guid.NewGuid();
                            answerTime.Agents.Add(agent);
                        }
                        answerTime.Agents.Add(agent);
                    }

                   

                    NewDataDTO data = provider.GetNewData(
                        currEnvelope.HardwareTree, 
                        agent.Id,
                        null);

                    hierarchyWriter.Write(data, agent);

                    metricWriter.WriteMetrics(currEnvelope.HardwareTree, currAgentSession.Id);
                }
            }
            dbContext.SaveChanges();
        }
    }
}