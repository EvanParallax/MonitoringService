using Common;
using Moq;
using NUnit.Framework;
using Server.DTOs;
using Server.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Tests.Tests
{
    [TestFixture]
    public class UtilsTestingTests
    {
        private IDataProvider sut;

        private HardwareTree defaultHwTree;

        [SetUp]
        public void CommonParts()
        {
            defaultHwTree = new HardwareTree();
            defaultHwTree.Sensors.Add(new SensorHW("1", "cpu_temp", 35.4f));
            defaultHwTree.Sensors.Add(new SensorHW("2", "gpu_temp", 83.2f));
            defaultHwTree.Sensors.Add(new SensorHW("3", "mb_temp", 32.4f));
        }

        [Test]
        public void EmptyDBGoodTree_ReturnAllHwSensors()
        {
            var expected = new List<SensorHW>();
            expected.Add(new SensorHW("1", "cpu_temp", 35.4f));
            expected.Add(new SensorHW("2", "gpu_temp", 83.2f));
            expected.Add(new SensorHW("3", "mb_temp", 32.4f));

            var tree = new HardwareTree();
            tree.Sensors.Add(new SensorHW("1", "cpu_temp", 35.4f));
            tree.Sensors.Add(new SensorHW("2", "gpu_temp", 83.2f));
            tree.Sensors.Add(new SensorHW("3", "mb_temp", 32.4f));

            var contextMock = new Mock<IReadOnlyDataContext>(MockBehavior.Strict);
            contextMock.Setup(a => a.Containers).Returns((new Container[0]).AsQueryable());
            contextMock.Setup(a => a.Sensors).Returns((new Sensor[0]).AsQueryable());
            contextMock.Setup(a => a.Agents).Returns((new Agent[0]).AsQueryable());

            sut = new DataProvider(contextMock.Object);

            var result = sut.GetNewContainers(tree, new Guid(), null).First();
                
            Assert.That(result.Sensors, Is.EqualTo(expected));
        }

        [Test]
        public void GoodDbGoodTreeEquals_EmptyNewList()
        {
            var tree = new HardwareTree();
            tree.Sensors.Add(new SensorHW("1", "cpu_temp", 35.4f));
            tree.Sensors.Add(new SensorHW("2", "gpu_temp", 83.2f));
            tree.Sensors.Add(new SensorHW("3", "mb_temp", 32.4f));

            var containers = new List<Container>();
            containers.Add(new Container() { AgentId = new Guid(), Id = new Guid(), ParentContainerId = null });

            var sensors = new List<Sensor>();
            sensors.Add(new Sensor() { Id = "1", Type = "cpu_temp", ContainerId = new Guid() });
            sensors.Add(new Sensor() { Id = "2", Type = "gpu_temp", ContainerId = new Guid() });
            sensors.Add(new Sensor() { Id = "3", Type = "mb_temp", ContainerId = new Guid() });


            var contextMock = new Mock<IReadOnlyDataContext>(MockBehavior.Strict);
            contextMock.Setup(a => a.Containers).Returns(containers.AsQueryable());
            contextMock.Setup(a => a.Sensors).Returns(sensors.AsQueryable());
            contextMock.Setup(a => a.Agents).Returns((new Agent[0]).AsQueryable());

            sut = new DataProvider(contextMock.Object);

            var result = sut.GetNewContainers(tree, new Guid(), null);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public void OneNodeDifference_NewNode()
        {
            var expected = new List<SensorHW>();
            expected.Add(new SensorHW("4", "winchester", 45.4f));

            var tree = new HardwareTree();
            tree.Sensors.Add(new SensorHW("1", "cpu_temp", 35.4f));
            tree.Sensors.Add(new SensorHW("2", "gpu_temp", 83.2f));
            tree.Sensors.Add(new SensorHW("3", "mb_temp", 32.4f));

            var anotherTree = new HardwareTree();
            anotherTree.Sensors.Add(new SensorHW("4", "winchester", 45.4f));
            tree.Subhardware.Add(anotherTree);

            var containers = new List<Container>();
            containers.Add(new Container() { AgentId = new Guid(), Id = new Guid(), ParentContainerId = null });

            var sensors = new List<Sensor>();
            sensors.Add(new Sensor() { Id = "1", Type = "cpu_temp", ContainerId = new Guid() });
            sensors.Add(new Sensor() { Id = "2", Type = "gpu_temp", ContainerId = new Guid() });
            sensors.Add(new Sensor() { Id = "3", Type = "mb_temp", ContainerId = new Guid() });


            var contextMock = new Mock<IReadOnlyDataContext>(MockBehavior.Strict);
            contextMock.Setup(a => a.Containers).Returns(containers.AsQueryable());
            contextMock.Setup(a => a.Sensors).Returns(sensors.AsQueryable());
            contextMock.Setup(a => a.Agents).Returns((new Agent[0]).AsQueryable());

            sut = new DataProvider(contextMock.Object);

            var result = sut.GetNewContainers(tree, new Guid(), null).First();

            Assert.That(result.Sensors, Is.EqualTo(expected));
        }
    }
}