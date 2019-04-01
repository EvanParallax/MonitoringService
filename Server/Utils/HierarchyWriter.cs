using Server.DTOs;
using System;
using System.Collections.Generic;

namespace Server.Utils
{
    public interface IHierarchyWriter
    {
        void WriteContainers(IEnumerable<ContainerDTO> containers, Agent agent);
    }

    public class HierarchyWriter : IHierarchyWriter
    {
        private IDataContext context;

        public HierarchyWriter(IDataContext ctx)
        {
            context = ctx;
        }

        public void WriteContainers(IEnumerable<ContainerDTO> containers, Agent agent)
        {
            foreach (var container in containers)
            {
                var CurrContainer = new Container()
                {
                    AgentId = agent.Id,
                    Id = Guid.NewGuid(),
                    ParentContainerId = container.ParentId
                };

                context.Containers.Add(CurrContainer);

                foreach(var sensor in container.Sensors)
                {
                    var curr_sensor = new Sensor()
                    {
                        Id = sensor.Id,
                        ContainerId = CurrContainer.Id,
                        Type = sensor.Type
                    };
                    context.Sensors.Add(curr_sensor);
                }
            }
            context.SaveChanges();
        }
    }
}