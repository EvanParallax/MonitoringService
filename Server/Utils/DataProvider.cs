using Common;
using NLog;
using Server.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Server.Utils
{
    public interface IDataProvider
    {
        NewDataDTO GetNewData(HardwareTree tree, Guid agentId, Guid? parentId);
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
            logger.Trace($"Entering {nameof(GetNewSensors)}");
            logger.Debug($"Current parent id = '{containerId}'");

            var sensors = dbContext.Sensors.Where(p => p.ContainerId == containerId).ToArray();
            foreach (var sensor in sensors)
                logger.Debug($"Found sensor '{sensor.ContainerId}:{sensor.Id}' type: {sensor.Type} in database");

            var buffTree = (from element in tree.Sensors
                            select element.Type );
            var enumerable = buffTree as string[] ?? buffTree.ToArray();
            foreach (var sensor in enumerable)
                logger.Debug($"Found sensor '{sensor}' in tree");

            var buffDb = (from element in sensors
                          select element.Type).AsEnumerable();

            var items = enumerable.Except(buffDb).AsEnumerable();
            foreach (var sensor in enumerable)
                logger.Debug($"Found new sensor '{sensor}' in tree");

            List<NewSensorDTO> newSensors = new List<NewSensorDTO>();

            foreach (var item in tree.Sensors)
            {
                if (items.Contains(item.Type))
                {
                        logger.Info($"New item '{item.Id}' will be added to DB");
                        newSensors.Add(new NewSensorDTO()
                        {
                            ContainerId = containerId,
                            Sensor = item
                        });
                }
                else
                {
                        logger.Debug($"Item '{item.Id}' already exists");
                }
            }

            logger.Trace($"Exiting {nameof(GetNewSensors)}");
            return newSensors;
        }

        private List<ContainerDTO> ConvertToList(HardwareTree tree, Guid? parentId)
        {
            logger.Trace($"Entering {new StackTrace().GetFrame(0).GetMethod().Name}");
            logger.Debug($"Parent id = '{parentId}'");

            List<ContainerDTO> containers = new List<ContainerDTO>();

            var container = new ContainerDTO()
            {
                DeviceName = tree.DeviceName,
                ContainerId = Guid.NewGuid(),
                ParentId = parentId,
                Sensors = tree.Sensors
            };

            containers.Add(container);
            logger.Info($"Adding container '{container.ContainerId}' to list");
            logger.Debug($"\twith sensors '{string.Join(",", container.Sensors.Select(s => s.Id))}'");

            foreach (var subhardware in tree.Subhardware)
            {
                containers.AddRange(ConvertToList(subhardware, container.ContainerId));
            }

            logger.Trace($"Exiting {new StackTrace().GetFrame(0).GetMethod().Name}");
            return containers;
        }

        private void GetNewContainers(HardwareTree tree, Guid agentId, Guid? parentId,NewDataDTO data)
        {
            logger.Trace($"Entering {new StackTrace().GetFrame(0).GetMethod().Name}");

            var dbcontainers = dbContext.Containers.Where(p => p.AgentId == agentId && p.ParentContainerId == parentId);

            if (dbcontainers.Count() == 0)
            {
                logger.Info("Empty DbSet Containers, creating new containers ");
                
                data.NewContainers.AddRange(ConvertToList(tree, parentId));

                logger.Trace($"Exiting {new StackTrace().GetFrame(0).GetMethod().Name}");

                return;
            }
                

            Guid resultId = Guid.Empty;
            bool needNewContainer = true;

            foreach (var container in dbcontainers)
            {
                if (container.AgentId == agentId && container.ParentContainerId == parentId)
                {
                    if (NotNeedNewContainer(container.Id, tree))
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
                GetNewContainers(sh, agentId, resultId, data);
            }

            logger.Trace($"Exiting {new StackTrace().GetFrame(0).GetMethod().Name}");

        }

        private bool NotNeedNewContainer(Guid id, HardwareTree tree)
        {
            var result = false;

            var container = dbContext.Containers.First(p => p.Id == id);

            if (container.DeviceName == tree.DeviceName)
                result = true;

            
            logger.Debug($"{nameof(NotNeedNewContainer)} result is {result}");
            return result;
        }

        public NewDataDTO GetNewData(HardwareTree tree, Guid agentId, Guid? parentId)
        {
            logger.Trace($"Entering {new StackTrace().GetFrame(0).GetMethod().Name}");

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
                logger.Info("New Container id {0}, pareint id {1}", item.ContainerId, item.ParentId);

                foreach (var sensor in item.Sensors)
                {
                    logger.Info("Sensor id {0}, type {1}, value {2}", 
                        sensor.Id, 
                        sensor.Type,
                        sensor.Value);
                }
            }

            foreach (var sensor in data.NewSensors)
            {
                logger.Info("Sensor id {0}, pareint id {1}, value {2}, container id {3}",
                    sensor.Sensor.Id,
                    sensor.Sensor.Type,
                    sensor.Sensor.Value, 
                    sensor.ContainerId);
            }

            if (data.NewContainers.Count == 0)
                logger.Info("No new containers");

            if (data.NewSensors.Count == 0)
                logger.Info("No new sensors");

            logger.Trace($"Exiting {new StackTrace().GetFrame(0).GetMethod().Name}");

            return data;

        }
    }
}