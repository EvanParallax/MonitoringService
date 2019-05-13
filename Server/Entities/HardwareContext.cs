using Server.Entities;
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
        IQueryable<Delay> Delays { get; }
        IQueryable<AgentDelay> AgentDelays { get; }
    }

    public interface IDataContext : IReadOnlyDataContext
    {
        new IDbSet<Agent> Agents { get; set; }
        new IDbSet<Container> Containers { get; set; }
        new IDbSet<Credential> Credentials { get; set; }
        new IDbSet<Metric> Metrics { get; set; }
        new IDbSet<Sensor> Sensors { get; set; }
        new IDbSet<Session> Sessions { get; set; }
        new IDbSet<Delay> Delays { get; set;}
        new IDbSet<AgentDelay> AgentDelays { get; set; }

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

        IQueryable<Delay> IReadOnlyDataContext.Delays => Delays;
        public IDbSet<Delay> Delays { get; set; }

        IQueryable<AgentDelay> IReadOnlyDataContext.AgentDelays => AgentDelays;
        public IDbSet<AgentDelay> AgentDelays { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Metric>()
                .HasKey(c => new { c.SessionId, c.SensorId });

            modelBuilder.Entity<AgentDelay>()
                .HasKey(c => new { c.AgentId, c.Date });
        }
    }

}