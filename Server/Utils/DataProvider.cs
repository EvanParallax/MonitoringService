using Common;
using NLog;
using Server.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Utils
{
    public interface IDataProvider
    {
        NewDataDTO GetNewData(HardwareTree tree, Guid agentId, Guid? parentId);
        //todo List<...> GetNewSensors(...);
    }

    public class DataProvider : IDataProvider
    {
        private readonly Logger logger;

        private readonly IReadOnlyDataContext dbContext;

        public DataProvider(IReadOnlyDataContext ctx)
        {
            logger = LogManager.GetCurrentClassLogger();
            dbContext = ctx;
        }

        private List<NewSensorDTO> GetNewSensors(HardwareTree tree, Guid containerId)
        {
            logger.Debug(" ");
            logger.Debug("Starting Converting new sensors to list  method");

            var sensors = dbContext.Sensors.Where(p => p.ContainerId == containerId);

            var buffTree = (from element in tree.Sensors
                            select element.Type );

            var buffDb = (from element in sensors
                          select element.Type).AsEnumerable();

            var items = buffTree.Except(buffDb).AsEnumerable();

            List<NewSensorDTO> newSensors = new List<NewSensorDTO>();

            foreach (var item in tree.Sensors)
                if(items.Contains(item.Type))
                    newSensors.Add(new NewSensorDTO()
                    {
                        ContainerId = containerId,
                        Sensor = item
                    });
            logger.Debug("Ending Converting new containers to list  method");
            logger.Debug(" ");
            return newSensors;
        }

        private List<ContainerDTO> ConvertToList(HardwareTree tree, Guid? parentId)
        {
            logger.Debug(" ");
            logger.Debug("Starting Converting new containers to list  method");

            List<ContainerDTO> containers = new List<ContainerDTO>();

            var container = new ContainerDTO()
            {
                ContainerId = Guid.NewGuid(),
                ParentId = parentId,
                Sensors = tree.Sensors
            };

            containers.Add(container);

            foreach (var subhardware in tree.Subhardware)
            {
                logger.Debug("Recursive call");
                containers.AddRange(ConvertToList(subhardware, container.ContainerId));
            }

            logger.Debug("Ending Converting new containers to list  method");
            logger.Debug(" ");
            return containers;
        }

        private void GetNewContainers(HardwareTree tree, Guid agentId, Guid? parentId,NewDataDTO data)
        {
            logger.Debug(" ");
            logger.Debug("Starting GetNewContainers method");

            var dbcontainers = dbContext.Containers.Where(p => p.AgentId == agentId && p.ParentContainerId == parentId);

            if (dbcontainers.Count() == 0)
            {
                logger.Debug("Empty DbSet Containers, creating new containers ");
                
                data.NewContainers.AddRange(ConvertToList(tree, parentId));

                return;

                logger.Debug("Ending GetNewContainers method");
                logger.Debug(" ");
            }
                

            Guid resultId = Guid.Empty;
            bool needNewContainer = true;

            foreach (var container in dbcontainers)
            {
                logger.Debug("Comparing containers");
                if (container.AgentId == agentId && container.ParentContainerId == parentId)
                {
                    logger.Debug("Need new Container: {0}", CompareNodes(container.Id, tree));
                    if (CompareNodes(container.Id, tree))
                    {
                        data.NewSensors.AddRange(GetNewSensors(tree, container.Id));
                        resultId = container.Id;
                        needNewContainer = false;
                        break;
                    }
                }
            }

            if(needNewContainer)
            {
                var buff = ConvertToList(tree, parentId);
                data.NewContainers.AddRange(buff);
                resultId = buff.First().ContainerId;
            }

            foreach (var sh in tree.Subhardware)
            {
                logger.Debug("Recursive call");
                GetNewContainers(sh, agentId, resultId, data);
            }

            logger.Debug("Ending GetNewContainers method");
            logger.Debug(" ");

        }

        private bool CompareNodes(Guid id, HardwareTree tree)
        {
            var sensors = dbContext.Sensors.Where(p => p.ContainerId == id).AsEnumerable();

            var buffTree = (from element in tree.Sensors
                          select new { element.Type, element.Id });

            var buffDb = (from element in sensors
                          select new { element.Type, element.Id }).AsEnumerable();

            if(buffTree.Intersect(buffDb).Count() == 0)
                return false;
            return true;
        }

        public NewDataDTO GetNewData(HardwareTree tree, Guid agentId, Guid? parentId)
        {
            logger.Debug(" ");
            logger.Debug("Starting GetNewData method");
            NewDataDTO data = new NewDataDTO()
            {
                NewSensors = new List<NewSensorDTO>(),
                NewContainers = new List<ContainerDTO>()
            };

            GetNewContainers(tree, agentId, parentId, data);

            data.NewContainers = data.NewContainers.Distinct().ToList();
            data.NewSensors = data.NewSensors.Distinct().ToList();

            foreach (var item in data.NewContainers)
            {
                logger.Debug("New Container id {0}, pareint id {1}", item.ContainerId, item.ParentId);

                foreach (var sensor in item.Sensors)
                {
                    logger.Debug("Sensor id {0}, type {1}, value {2}", 
                        sensor.Id, 
                        sensor.Type,
                        sensor.Value);
                }
            }

            foreach (var sensor in data.NewSensors)
            {
                logger.Debug("Sensor id {0}, pareint id {1}, value {2}, container id {3}",
                    sensor.Sensor.Id,
                    sensor.Sensor.Type,
                    sensor.Sensor.Value, 
                    sensor.ContainerId);
            }

            if (data.NewContainers.Count == 0)
                logger.Debug("No new containers");

            if (data.NewSensors.Count == 0)
                logger.Debug("No new sensors");

            logger.Debug("Ending GetNewData method");
            logger.Debug(" ");

            return data;

        }
    }
}