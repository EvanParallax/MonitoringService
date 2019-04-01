
using Agent.Utils;
using Common;
using System;
using System.Web.Http;

namespace Agent.Controllers
{
    public class ValuesController : ApiController
    {
        private readonly ISensorWatcher watcher;

        public ValuesController(ISensorWatcher sensorWatcher)
        {
            watcher = sensorWatcher;
        }

        [HttpGet]
        public Envelope Get()
        {
            var hardwareTree = watcher.GetSensorsData();

            return new Envelope
            {
                Header = new Header
                {
                    AgentId = Guid.NewGuid(), // todo: remove AgentId
                    AgentTime = DateTime.Now,
                    ErrorMsg = hardwareTree == null ? "No data available" : ""
                },
                HardwareTree = hardwareTree
            };
        }
    }
}
