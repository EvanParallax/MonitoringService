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
        public AgentsList GetAgents()
        {
            List<AgentDTO> agents = new List<AgentDTO>();
            foreach (var item in ctx.Agents)
            {
                var creds = ctx.Credentials.FirstOrDefault(c => c.Id == item.CredId);
                AgentDTO buff = new AgentDTO()
                {
                    Id = item.Id,
                    Endpoint = item.Endpoint,
                    OsType = item.OsType,
                    AgentVersion = item.AgentVersion,
                    Login = creds.Login,
                    Password = creds.Password
                };
                agents.Add(buff);
            }

            AgentsList agentsList = new AgentsList();
            agentsList.Agents = agents;
            return agentsList;
        }

        [HttpPost]
        public IHttpActionResult AddAgent(AgentDTO agent)
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
                CredId = cred.Id,
                Endpoint = agent.Endpoint,
                OsType = agent.OsType,
                AgentVersion = agent.AgentVersion
            };
            ctx.Credentials.Add(cred);
            ctx.Agents.Add(item);
            ctx.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [Route("enable/{id}")]
        public IHttpActionResult EnableAgent(string id)
        {
            var identifier = Guid.Parse(id);
            var agent = ctx.Agents.Where(a => a.Id == identifier).FirstOrDefault();
            if (agent != null)
            {
                agent.IsEnabled = true;
                ctx.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

        [HttpPost]
        [Route("disable/{id}")]
        public IHttpActionResult DisableAgent(string id)
        {
            var identifier = Guid.Parse(id);
            var agent = ctx.Agents.Where(a => a.Id == identifier).FirstOrDefault();
            if (agent != null)
            {
                agent.IsEnabled = false;
                ctx.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

        [HttpGet]
        [Route("delete/{id}")]
        public IHttpActionResult DeleteAgent(string id)
        {
            var identifier = Guid.Parse(id);
            var agent = ctx.Agents.Where(a => a.Id == identifier).FirstOrDefault();
            if (agent == null)
                return NotFound();

            ctx.Agents.Remove(agent);

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

            return Ok();
        }
    }
}
