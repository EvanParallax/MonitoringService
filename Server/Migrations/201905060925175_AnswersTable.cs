namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnswersTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AnswerTimes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Time = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AnswerTimeAgents",
                c => new
                    {
                        AnswerTime_Id = c.Guid(nullable: false),
                        Agent_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.AnswerTime_Id, t.Agent_Id })
                .ForeignKey("dbo.AnswerTimes", t => t.AnswerTime_Id, cascadeDelete: true)
                .ForeignKey("dbo.Agents", t => t.Agent_Id, cascadeDelete: true)
                .Index(t => t.AnswerTime_Id)
                .Index(t => t.Agent_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AnswerTimeAgents", "Agent_Id", "dbo.Agents");
            DropForeignKey("dbo.AnswerTimeAgents", "AnswerTime_Id", "dbo.AnswerTimes");
            DropIndex("dbo.AnswerTimeAgents", new[] { "Agent_Id" });
            DropIndex("dbo.AnswerTimeAgents", new[] { "AnswerTime_Id" });
            DropTable("dbo.AnswerTimeAgents");
            DropTable("dbo.AnswerTimes");
        }
    }
}
