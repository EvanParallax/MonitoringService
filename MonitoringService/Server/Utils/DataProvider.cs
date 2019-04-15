using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Utils
{
    public interface IDataProvider
    {
        Session WriteSession(Agent agent);

        void WriteContainer(HardwareTree tree, IDataContext ctx, Guid agentId, Guid parentId);
    }

    public class DataProvider : IDataProvider
    {
        public void WriteContainer(HardwareTree tree, IDataContext ctx, Guid agentId, Guid parentId)
        {
            foreach (var container in ctx.Containers)
            {
                if (container.AgentId == agentId && container.ParentContainerId == parentId)
                    if (!CompareNodes(ctx, container, tree)) {
                        var newContainer = new Container()
                        {
                            Id = Guid.NewGuid(),
                            AgentId = agentId,
                            ParentContainerId = parentId
                        };
                        ctx.Containers.Add(newContainer);
                        foreach (var sh in tree.Subhardware)
                            WriteContainer(sh, ctx, agentId, newContainer.Id);
                    }
                foreach (var sh in tree.Subhardware)
                    WriteContainer(sh, ctx, agentId, container.Id);
            }

        }

        private bool CompareNodes(IDataContext ctx, Container container, HardwareTree tree)
        {
            var sensors = ctx.Sensors.Where(p => p.ContainerId == container.Id);
            if (sensors.Count() != tree.Sensors.Count)
                return false;

            return true;
        }

        public Session WriteSession(Agent agent)
        {
            throw new NotImplementedException();
        }
    }
}