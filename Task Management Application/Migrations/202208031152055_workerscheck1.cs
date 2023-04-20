namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class workerscheck1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkTasks", "Forfeited", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WorkTasks", "Forfeited");
        }
    }
}
