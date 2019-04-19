namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReInitial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Containers", "DeviceName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Containers", "DeviceName");
        }
    }
}
