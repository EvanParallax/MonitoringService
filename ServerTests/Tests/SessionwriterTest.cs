using Common;
using Moq;
using NUnit.Framework;
using Server.Utils;
using ServerTests.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTests.Tests
{
    [TestFixture]
    public class SessionwriterTest
    {
        private SessionWriter sut;

        private Mock<IDataContext> fakeContext;

        private Mock<IDataReceiver> fakeReceiver;


        private Envelope Func()
        {
            Random rand = new Random();

            var tree = new HardwareTree();
            tree.Sensors.Add(new SensorHW("1", "PCH", rand.Next(30, 50)));

            var cpuTree = new HardwareTree();
            cpuTree.Sensors.Add(new SensorHW("2", "cpu_temp", rand.Next(30, 80)));

            var gpuTree = new HardwareTree();
            gpuTree.Sensors.Add(new SensorHW("3", "gpu_temp", rand.Next(40, 88)));

            var hddTree = new HardwareTree();
            hddTree.Sensors.Add(new SensorHW("4", "hdd_temp", rand.Next(25, 35)));
            var hdddTree = new HardwareTree();
            hdddTree.Sensors.Add(new SensorHW("66", "hd2d_temp", rand.Next(25, 35)));

            tree.Subhardware.Add(cpuTree);
            tree.Subhardware.Add(gpuTree);
            tree.Subhardware.Add(hddTree);
            tree.Subhardware.Add(hdddTree);

            var envelope = new Envelope
            {
                Header = new Header
                {
                    AgentId = Guid.NewGuid(),
                    AgentTime = DateTime.Now,
                    ErrorMsg = tree == null ? "No data available" : ""
                },
                HardwareTree = tree
            };

            return envelope;
        }


        [SetUp]
        public void CommonParts()
        {

            FakeDbSet <Agent> agents = new FakeDbSet<Agent>();
            FakeDbSet<Credential> creds = new FakeDbSet<Credential>();
            FakeDbSet<Container> containers = new FakeDbSet<Container>();
            FakeDbSet<Sensor> sensors = new FakeDbSet<Sensor>();
            FakeDbSet<Metric> metrics = new FakeDbSet<Metric>();
            FakeDbSet<Session> sessions = new FakeDbSet<Session>();

            fakeContext = new Mock<IDataContext>();
            fakeContext.Setup(a => a.Containers).Returns(containers);
            fakeContext.Setup(a => a.Sensors).Returns(sensors);
            fakeContext.Setup(a => a.Agents).Returns(agents);
            fakeContext.Setup(a => a.Metrics).Returns(metrics);
            fakeContext.Setup(a => a.Sessions).Returns(sessions);
            fakeContext.Setup(a => a.Credentials).Returns(creds);

            fakeReceiver = new Mock<IDataReceiver>();
            fakeReceiver.Setup(a => a.GetDataAsync(It.IsAny<string>())).Returns(new Task<Envelope>(Func));

            fakeContext.Object.Agents.Add(new Agent()
            {
                Id = Guid.NewGuid(),
                CredId = Guid.NewGuid(),
                Endpoint = "http://localhost:51221/api/Values",
                OsType = "win",
                AgentVersion = "0.1"
            });

            fakeContext.Object.SaveChanges();
        }

        [Test]
        public void EmptyDB_WriteAll()
        {
            sut = new SessionWriter(fakeContext.Object,
                                    new DataReceiver(), 
                                    new DataProvider(fakeContext.Object), 
                                    new HierarchyWriter(fakeContext.Object), 
                                    new MetricWriter(fakeContext.Object));

            sut.WriteSession();

            Assert.That(fakeContext.Object.Sessions.Count, Is.EqualTo(1));
            Assert.That(fakeContext.Object.Containers.Count, Is.EqualTo(5));
            Assert.That(fakeContext.Object.Sensors.Count, Is.EqualTo(5));
            Assert.That(fakeContext.Object.Metrics.Count, Is.EqualTo(5));
        }

        [Test]
        public void NotEmptyDB_WriteSessionAndMetrics()
        {
            sut = new SessionWriter(fakeContext.Object,
                                    new DataReceiver(), 
                                    new DataProvider(fakeContext.Object),
                                    new HierarchyWriter(fakeContext.Object),
                                    new MetricWriter(fakeContext.Object));

            sut.WriteSession();
            //fakeContext.Object.SaveChanges();
            sut.WriteSession();
            //fakeContext.Object.SaveChanges();

            Assert.That(fakeContext.Object.Sessions.Count, Is.EqualTo(2));
            Assert.That(fakeContext.Object.Containers.Count, Is.EqualTo(5));
            Assert.That(fakeContext.Object.Sensors.Count, Is.EqualTo(5));
            Assert.That(fakeContext.Object.Metrics.Count, Is.EqualTo(10));
        }
    }
}
