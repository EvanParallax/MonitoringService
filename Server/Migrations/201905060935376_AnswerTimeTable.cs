namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AnswerTimeTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AnswerTimes", "AnswerDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.AnswerTimes", "Delay", c => c.Int(nullable: false));
            DropColumn("dbo.AnswerTimes", "Time");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AnswerTimes", "Time", c => c.DateTime(nullable: false));
            DropColumn("dbo.AnswerTimes", "Delay");
            DropColumn("dbo.AnswerTimes", "AnswerDate");
        }
    }
}
