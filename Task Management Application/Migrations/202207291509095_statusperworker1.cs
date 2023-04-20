namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class statusperworker1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkTasks", "Finished", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WorkTasks", "Finished");
        }
    }
}
