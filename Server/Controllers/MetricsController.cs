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
    }
}
