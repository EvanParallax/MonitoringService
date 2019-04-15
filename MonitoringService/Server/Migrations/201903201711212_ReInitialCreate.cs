namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReInitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Agents",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        CredId = c.Guid(nullable: false),
                        Endpoint = c.String(),
                        OsType = c.String(),
                        AgentVersion = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Containers",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AgentId = c.Guid(nullable: false),
                        ParentContainerId = c.Guid(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Credentials",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Login = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Metrics",
                c => new
                    {
                        SessionId = c.Guid(nullable: false),
                        SensorId = c.Guid(nullable: false),
                        Valued = c.Single(nullable: false),
                    })
                .PrimaryKey(t => new { t.SessionId, t.SensorId });
            
            CreateTable(
                "dbo.Sensors",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        ContainerId = c.Guid(nullable: false),
                        Type = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Sessions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        AgentId = c.Guid(nullable: false),
                        AgentTime = c.DateTime(nullable: false),
                        ServerTime = c.DateTime(nullable: false),
                        Error = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Sessions");
            DropTable("dbo.Sensors");
            DropTable("dbo.Metrics");
            DropTable("dbo.Credentials");
            DropTable("dbo.Containers");
            DropTable("dbo.Agents");
        }
    }
}
