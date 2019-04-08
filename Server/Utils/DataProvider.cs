using Common;
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
        private readonly IReadOnlyDataContext dbContext;

        public DataProvider(IReadOnlyDataContext ctx)
        {
            dbContext = ctx;
        }

        private List<NewSensorDTO> GetNewSensors(HardwareTree tree, Guid containerId)
        {
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
            return newSensors;
        }

        private List<ContainerDTO> ConvertToList(HardwareTree tree, Guid? parentId)
        {
            List<ContainerDTO> containers = new List<ContainerDTO>();

            var container = new ContainerDTO()
            {
                ContainerId = Guid.NewGuid(),
                ParentId = parentId,
                Sensors = tree.Sensors
            };

            containers.Add(container);

            foreach (var subhardware in tree.Subhardware)
                containers.AddRange(ConvertToList(subhardware, container.ContainerId));

            return containers;
        }

        private void GetNewContainers(HardwareTree tree, Guid agentId, Guid? parentId,NewDataDTO data)
        {
            var dbcontainers = dbContext.Containers.Where(p => p.AgentId == agentId && p.ParentContainerId == parentId);

            if (dbcontainers.Count() == 0)
                data.NewContainers.AddRange(ConvertToList(tree, parentId));

            Guid resultId = Guid.Empty;
            bool needNewContainer = true;

            foreach (var container in dbcontainers)
            {
                if (container.AgentId == agentId && container.ParentContainerId == parentId)
                {
                    if (CompareNodes(container.Id, tree))
                    {
                        data.NewSensors.AddRange(GetNewSensors(tree, container.Id));
                        resultId = container.Id;
                        needNewContainer = false;
                        break;
                    }

                    // if (!CompareNodes(container.Id, tree))
                    //   data.NewContainers.AddRange(ConvertToList(tree, parentId));
                    // else
                    //  data.NewSensors.AddRange(GetNewSensors(tree, container.Id));

                    //foreach (var sh in tree.Subhardware)
                        //GetNewContainers(sh, agentId, resultId, data);
                }
            }

            if(needNewContainer)
            {
                var buff = ConvertToList(tree, parentId);
                data.NewContainers.AddRange(buff);
                resultId = buff.First().ContainerId;
            }

            foreach (var sh in tree.Subhardware)
                GetNewContainers(sh, agentId, resultId, data);

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
            NewDataDTO data = new NewDataDTO()
            {
                NewSensors = new List<NewSensorDTO>(),
                NewContainers = new List<ContainerDTO>()
            };

            GetNewContainers(tree, agentId, parentId, data);

            data.NewContainers = data.NewContainers.Distinct().ToList();
            data.NewSensors = data.NewSensors.Distinct().ToList();
            return data;
        }
    }
}