using Common;
using Newtonsoft.Json;
using Server.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Server.Controllers
{
    public class ServerController : ApiController
    {
        private IDataContext ctx;

        public ServerController(IDataContext ctx)
        {
            this.ctx = ctx;
        }

        [HttpPost]
        public IEnumerable<Agent> AddAgent(Agent agent)
        {
            //var agent = new Agent()
            //{
            //    Id = Guid.NewGuid(),
            //    Endpoint = "localhost"
            //};
            //ctx.Agents.Add(agent);
            //ctx.SaveChanges();
            return ctx.Agents;
            
        }

        
        public IEnumerable<MetricDTO> GetMetrics(string id)
        {
            Guid agentId = new Guid(id);
            var result = from Agent in ctx.Agents 
                         where Agent.Id == agentId
                         join Session in ctx.Sessions on Agent.Id equals Session.AgentId
                         join Metric in ctx.Metrics on Session.Id equals Metric.SessionId
                         join Sensor in ctx.Sensors on Metric.SensorId equals Sensor.Id
                         select new MetricDTO()
                         {
                             Session = Session.AgentTime,
                             Stype = Sensor.Type,
                             Svalue = Metric.Value
                         };

            return result.AsEnumerable();
        }
        
        [HttpGet]
        public IEnumerable<AgentDTO> GetAgents()
        {
            List<AgentDTO> agents = new List<AgentDTO>();
            foreach (var item in ctx.Agents)
            {
                AgentDTO buff = new AgentDTO()
                {
                    Id = item.Id,
                    Endpoint = item.Endpoint,
                    OsType = item.OsType,
                    AgentVersion = item.AgentVersion
                };
                agents.Add(buff);
            }
            return agents;
        }

        [HttpPost]
        public void AddAgent(AgentDTO agent)
        {
            Agent item = new Agent()
            {
                Id = Guid.NewGuid(),
                CredId = new Guid(),
                Endpoint = agent.Endpoint,
                OsType = agent.OsType,
                AgentVersion = agent.AgentVersion
            };
            ctx.Agents.Add(item);
            ctx.SaveChanges();
        }
    }
}
