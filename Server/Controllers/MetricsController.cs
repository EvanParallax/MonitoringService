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
        //[Route("GetMetrics/{id}")]
        public MetricsList GetMetrics(string id)
        {
            var result = from agent in ctx.Agents

                         where agent.Id == new Guid(id)

                         join session in ctx.Sessions on agent.Id equals session.AgentId

                         join metric in ctx.Metrics on session.Id equals metric.SessionId

                         join sensor in ctx.Sensors on metric.SensorId equals sensor.Id

                         join agentDelay in ctx.AgentDelays on agent.Id equals agentDelay.AgentId

                         join delay in ctx.Delays on agentDelay.DelayId equals delay.Id

                         select new MetricDTO()
                         {
                             Session = session.AgentTime,
                             Delay = delay.Value,
                             Stype = sensor.Type,
                             Svalue = metric.Value
                         };
            MetricsList metrics = new MetricsList();
            metrics.Metrics = result.ToList(); ;
            return metrics;
        }
    }
}
