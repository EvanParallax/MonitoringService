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

        [HttpGet]
        public IEnumerable<Agent> GetAgents()
        {
            return ctx.Agents;
        }

        /*
        public Cover Get()
        {
            using (HardwareContext db = new HardwareContext())
            {
                var result = from Agent in db.Agents
                             join Session in db.Sessions on Agent.Id equals Session.AgentId
                             join Metric in db.Metrics on Session.Id equals Metric.SessionId
                             join Sensor in db.Sensors on Metric.SensorId equals Sensor.Id
                             select new
                             {
                                 agent = Agent.Id,
                                 session = Session.AgentTime,
                                 stype = Sensor.Type,
                                 svalue = Metric.Valued
                             };
            }
            
      
        }*/
    }
}
