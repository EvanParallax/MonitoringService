using Common;
using Server.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Server.Controllers
{
    public class AgentController : ApiController
    {
        private IDataContext ctx;

        public AgentController(IDataContext ctx)
        {
            this.ctx = ctx;
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
            var cred = new Credential()
            {
                Id = Guid.NewGuid(),
                Login = agent.Login,
                Password = agent.Password
            };

            Agent item = new Agent()
            {
                Id = Guid.NewGuid(),
                CredId = new Guid(), 
                Endpoint = agent.Endpoint,
                OsType = agent.OsType,
                AgentVersion = agent.AgentVersion
            };
            ctx.Credentials.Add(cred);
            ctx.Agents.Add(item);
            ctx.SaveChanges();
        }

        [HttpPost]
        public void EnableAgent(string endpoint)
        {
            var agent = ctx.Agents.Where(a => a.Endpoint == endpoint).FirstOrDefault();
            if(agent != null)
                agent.IsEnabled = true;
        }

        [HttpPost]
        public void DisableAgent(string endpoint)
        {
            var agent = ctx.Agents.Where(a => a.Endpoint == endpoint).FirstOrDefault();
            if (agent != null)
                agent.IsEnabled = false;
        }

        [HttpPost]
        public void DeleteAgent(string endpoint)
        {
            var agent = ctx.Agents.Where(a => a.Endpoint == endpoint).FirstOrDefault();
            if(agent != null)
                ctx.Agents.Remove(agent);
            else
            {
                return;
            }

            var cred = ctx.Credentials.Where(c => c.Id == agent.CredId).FirstOrDefault();
            ctx.Credentials.Remove(cred);

            var sessions = ctx.Sessions.Where(s => s.AgentId == agent.Id);
            foreach (var item in sessions)
            {
                var metrics = ctx.Metrics.Where(m => m.SessionId == item.Id);
                foreach (var metric in metrics)
                    ctx.Metrics.Remove(metric);
                ctx.Sessions.Remove(item);
            }

            var containers = ctx.Containers.Where(c => c.AgentId == agent.Id);
            foreach (var item in containers)
            {
                var sensors = ctx.Sensors.Where(s => s.ContainerId == item.Id);
                foreach (var sensor in sensors)
                    ctx.Sensors.Remove(sensor);

                ctx.Containers.Remove(item);
            }

            ctx.SaveChanges();
        }
    }
}
