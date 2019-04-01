using Common;
using System;

namespace Server.Utils
{
    public interface IMetricWriter
    {
        void WriteMetrics(HardwareTree tree, Guid sessionId);
    }

    public class MetricWriter : IMetricWriter
    {
        private readonly IDataContext dataContext;

        public MetricWriter(IDataContext ctx)
        {
            dataContext = ctx;
        }

        public void WriteMetrics(HardwareTree tree, Guid sessionId)
        {
            foreach (var sensor in tree.Sensors)
            {
                var metric = new Metric()
                {
                    SessionId = sessionId,
                    SensorId = sensor.Id,
                    Value = sensor.Value
                };

                dataContext.Metrics.Add(metric);
            }
                foreach (var sh in tree.Subhardware)
                    WriteMetrics(sh, sessionId);
            
            dataContext.SaveChanges();
        }
    }
}