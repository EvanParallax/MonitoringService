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

        [SetUp]
        public void CommonParts()
        {
            FakeDbSet<Agent> agents = new FakeDbSet<Agent>();
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
