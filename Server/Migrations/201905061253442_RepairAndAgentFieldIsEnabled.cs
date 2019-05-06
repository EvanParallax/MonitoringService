namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RepairAndAgentFieldIsEnabled : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Agents", "IsEnabled", c => c.Boolean(nullable: false));
            DropColumn("dbo.AnswerTimes", "AnswerDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AnswerTimes", "AnswerDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Agents", "IsEnabled");
        }
    }
}
