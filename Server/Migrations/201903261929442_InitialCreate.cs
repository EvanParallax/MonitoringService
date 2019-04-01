namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.Metrics");
            AddColumn("dbo.Metrics", "Value", c => c.Single(nullable: false));
            AlterColumn("dbo.Metrics", "SensorId", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.Metrics", new[] { "SessionId", "SensorId" });
            DropColumn("dbo.Metrics", "Valued");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Metrics", "Valued", c => c.Single(nullable: false));
            DropPrimaryKey("dbo.Metrics");
            AlterColumn("dbo.Metrics", "SensorId", c => c.Guid(nullable: false));
            DropColumn("dbo.Metrics", "Value");
            AddPrimaryKey("dbo.Metrics", new[] { "SessionId", "SensorId" });
        }
    }
}
