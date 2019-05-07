using Common;
using Server.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Server.Controllers
{
    public class MetricsController : ApiController
    {
        private IReadOnlyDataContext ctx;

        public MetricsController(IDataContext ctx)
        {
            this.ctx = ctx;
        }

        [HttpGet]
        public IEnumerable<MetricDTO> GetMetrics(string id)
        {
            Guid agentId = new Guid(id);
            var result = from agent in ctx.Agents
                         where agent.Id == agentId
                         join session in ctx.Sessions on agent.Id equals session.AgentId
                         join metric in ctx.Metrics on session.Id equals metric.SessionId
                         join sensor in ctx.Sensors on metric.SensorId equals sensor.Id
                         select new MetricDTO()
                         {
                             Session = session.AgentTime,
                             Stype = sensor.Type,
                             Svalue = metric.Value
                         };

            return result.AsEnumerable();
        }
    }
}
