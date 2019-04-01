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
        private readonly IDataContext context;

        public HierarchyWriter(IDataContext ctx)
        {
            context = ctx;
        }

        public void WriteContainers(IEnumerable<ContainerDTO> containers, Agent agent)
        {
            foreach (var container in containers)
            {
                var currentContainer = new Container // naming!
                {
                    AgentId = agent.Id,
                    Id = Guid.NewGuid(),
                    ParentContainerId = container.ParentId
                };

                context.Containers.Add(currentContainer);

                foreach(var sensor in container.Sensors)
                {
                    var currentSensor = new Sensor // naming!
                    {
                        Id = sensor.Id,
                        ContainerId = currentContainer.Id,
                        Type = sensor.Type
                    };
                    context.Sensors.Add(currentSensor);
                }
            }
            context.SaveChanges();
        }
    }
}