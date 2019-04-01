using System;
using System.Data.Entity;
using System.Linq;

namespace Server.Utils
{
    public interface IReadOnlyDataContext : IDisposable
    {
        IQueryable<Agent> Agents { get; }
        IQueryable<Container> Containers { get; }
        IQueryable<Credential> Credentials { get; }
        IQueryable<Metric> Metrics { get; }
        IQueryable<Sensor> Sensors { get; }
        IQueryable<Session> Sessions { get; }
    }

    public interface IDataContext : IReadOnlyDataContext
    {
        new IDbSet<Agent> Agents { get; set; }
        new IDbSet<Container> Containers { get; set; }
        new IDbSet<Credential> Credentials { get; set; }
        new IDbSet<Metric> Metrics { get; set; }
        new IDbSet<Sensor> Sensors { get; set; }
        new IDbSet<Session> Sessions { get; set; }

        int SaveChanges();
    }

    public class DataContext : DbContext, IDataContext
    {
        IQueryable<Agent> IReadOnlyDataContext.Agents => Agents;
        public IDbSet<Agent> Agents { get; set; }

        IQueryable<Container> IReadOnlyDataContext.Containers => Containers;
        public IDbSet<Container> Containers { get; set; }

        IQueryable<Credential> IReadOnlyDataContext.Credentials => Credentials;
        public IDbSet<Credential> Credentials { get; set; }

        IQueryable<Metric> IReadOnlyDataContext.Metrics => Metrics;
        public IDbSet<Metric> Metrics { get; set; }

        IQueryable<Sensor> IReadOnlyDataContext.Sensors => Sensors;
        public IDbSet<Sensor> Sensors { get; set; }

        IQueryable<Session> IReadOnlyDataContext.Sessions => Sessions;
        public IDbSet<Session> Sessions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Metric>()
                .HasKey(c => new { c.SessionId, c.SensorId });
        }
    }

}