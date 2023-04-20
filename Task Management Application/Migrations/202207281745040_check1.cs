namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class check1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkTasks", "SupervisorUserName", c => c.String());
            DropColumn("dbo.WorkTasks", "CreatorUserName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WorkTasks", "CreatorUserName", c => c.String());
            DropColumn("dbo.WorkTasks", "SupervisorUserName");
        }
    }
}
