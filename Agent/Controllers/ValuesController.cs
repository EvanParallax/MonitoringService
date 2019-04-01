
using Agent.Utils;
using Common;
using System;
using System.Web.Http;

namespace Agent.Controllers
{
    public class ValuesController : ApiController
    {
        private ISensorWatcher watcher;

        public ValuesController(ISensorWatcher sensorWatcher)
        {
            watcher = sensorWatcher;
        }

        [HttpGet]
        public Envelope Get()
        {
            var envelope = watcher.GetSensorsData();
            return envelope;
        }
    }
}
