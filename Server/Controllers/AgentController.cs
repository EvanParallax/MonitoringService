using Common;
using Server.Utils;
using System;
using System.Collections.Generic;
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
            Agent item = new Agent()
            {
                Id = Guid.NewGuid(),
                CredId = new Guid(), //todo: save credentials
                Endpoint = agent.Endpoint,
                OsType = agent.OsType,
                AgentVersion = agent.AgentVersion
            };
            ctx.Agents.Add(item);
            ctx.SaveChanges();
        }
    }
}
