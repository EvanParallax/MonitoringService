﻿using Common;
using Server.DTOs;
using System;
using System.Collections.Generic;

namespace Server.Utils
{
    public class SessionWriter : IDisposable
    {
        private readonly IDataContext dbContext;
        private readonly IDataReciever receiver;
        private readonly IDataProvider provider;
        private readonly IHierarchyWriter hierarchyWriter;
        private readonly IMetricWriter metricWriter;

        public SessionWriter(IDataContext ctx, IDataReciever rcvr, IDataProvider prvdr, IHierarchyWriter hWriter, IMetricWriter mWriter)
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
                Envelope currEnvelope = receiver.GetDataAsync(agent.Endpoint).Result;
                Session currAgentSession = new Session()
                {
                    AgentId = agent.Id,
                    Id = Guid.NewGuid(),
                    ServerTime = DateTime.Now,
                    AgentTime = currEnvelope.Header.AgentTime,
                    Error = currEnvelope.Header.ErrorMsg
                };

                dbContext.Sessions.Add(currAgentSession);

                if(currEnvelope.HardwareTree != null)
                {
                    List<ContainerDTO> newContainers = provider.GetNewContainers(
                        currEnvelope.HardwareTree, 
                        agent.Id,
                        null);

                    hierarchyWriter.WriteContainers(newContainers, agent);

                    metricWriter.WriteMetrics(currEnvelope.HardwareTree, currAgentSession.Id);
                }
            }
            dbContext.SaveChanges();
        }

    }
}