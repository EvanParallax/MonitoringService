using Server.DTOs;
using System;
using System.Collections.Generic;

namespace Server.Utils
{
    public interface IHierarchyWriter
    {
        void Write(NewDataDTO data, Guid agentId);
    }

    public class HierarchyWriter : IHierarchyWriter
    {
        private readonly IDataContext context;

        public HierarchyWriter(IDataContext ctx)
        {
            context = ctx;
        }

        public void Write(NewDataDTO data, Guid agentId)
        {
            foreach (var item in data.NewSensors)
            {
                var currentSensor = new Sensor // naming!
                {
                    Id = item.Sensor.Id,
                    ContainerId = item.ContainerId,
                    Type = item.Sensor.Type
                };
                context.Sensors.Add(currentSensor);
            }

            foreach (var container in data.NewContainers)
            {
                var currentContainer = new Container // naming!
                {
                    DeviceName = container.DeviceName,
                    AgentId = agentId,
                    Id = container.ContainerId,
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