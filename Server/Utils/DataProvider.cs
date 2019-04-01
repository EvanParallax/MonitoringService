using Common;
using Server.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Utils
{
    public interface IDataProvider
    {
        List<ContainerDTO> GetNewContainers(HardwareTree tree, Guid agentId, Guid? parentId);
        //todo List<...> GetNewSensors(...);
    }

    public class DataProvider : IDataProvider
    {
        private readonly IReadOnlyDataContext dbContext;

        public DataProvider(IReadOnlyDataContext ctx)
        {
            dbContext = ctx;
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

        public List<ContainerDTO> GetNewContainers(HardwareTree tree, Guid agentId, Guid? parentId)
        {
            var dbcontainers = dbContext.Containers.Where(p => p.AgentId == agentId && p.ParentContainerId == parentId);

            if (dbcontainers.Count() == 0)
                return ConvertToList(tree, parentId);

            List<ContainerDTO> containers = new List<ContainerDTO>();

            foreach (var container in dbcontainers)
            {
                if (container.AgentId == agentId && container.ParentContainerId == parentId)
                    if (!CompareNodes(container, tree)) {
                        return ConvertToList(tree, parentId);
                    }

                foreach (var sh in tree.Subhardware)
                    containers.AddRange(GetNewContainers(sh, agentId, container.Id));
            }
            return containers;

        }

        private bool CompareNodes(Container container, HardwareTree tree)
        {
            var sensors = dbContext.Sensors.Where(p => p.ContainerId == container.Id);
            if (sensors.Count() != tree.Sensors.Count)
                return false;

            var buffTree = (from element in tree.Sensors
                          select new { element.Type, element.Id });

            var buffDb = (from element in sensors
                          select new { element.Type, element.Id }).AsEnumerable();

            if(buffTree.Except(buffDb).Count() == 0)
                return true;
            return false;
        }
    }
}