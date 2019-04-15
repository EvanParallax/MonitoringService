
using Agent.Utils;
using Common;
using NLog;
using System;
using System.Web.Http;

namespace Agent.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly Logger logger = LogManager.GetCurrentClassLogger();

        private ISensorWatcher watcher;

        public ValuesController(ISensorWatcher sensorWatcher)
        {
            watcher = sensorWatcher;
        }

        [HttpGet]
        public Envelope Get()
        {
            var envelope = watcher.GetSensorsData();
            logger.Info("returning envelope ");
            return envelope;
        }
    }
}
