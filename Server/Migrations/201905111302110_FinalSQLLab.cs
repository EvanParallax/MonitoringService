namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FinalSQLLab : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AnswerTimeAgents", "AnswerTime_Id", "dbo.AnswerTimes");
            DropForeignKey("dbo.AnswerTimeAgents", "Agent_Id", "dbo.Agents");
            DropIndex("dbo.AnswerTimeAgents", new[] { "AnswerTime_Id" });
            DropIndex("dbo.AnswerTimeAgents", new[] { "Agent_Id" });
            CreateTable(
                "dbo.AgentDelays",
                c => new
                    {
                        AgentId = c.Guid(nullable: false),
                        Date = c.DateTime(nullable: false),
                        DelayId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.AgentId, t.Date });
            
            CreateTable(
                "dbo.Delays",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Value = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.AnswerTimes");
            DropTable("dbo.AnswerTimeAgents");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AnswerTimeAgents",
                c => new
                    {
                        AnswerTime_Id = c.Guid(nullable: false),
                        Agent_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.AnswerTime_Id, t.Agent_Id });
            
            CreateTable(
                "dbo.AnswerTimes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Delay = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("dbo.Delays");
            DropTable("dbo.AgentDelays");
            CreateIndex("dbo.AnswerTimeAgents", "Agent_Id");
            CreateIndex("dbo.AnswerTimeAgents", "AnswerTime_Id");
            AddForeignKey("dbo.AnswerTimeAgents", "Agent_Id", "dbo.Agents", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AnswerTimeAgents", "AnswerTime_Id", "dbo.AnswerTimes", "Id", cascadeDelete: true);
        }
    }
}
