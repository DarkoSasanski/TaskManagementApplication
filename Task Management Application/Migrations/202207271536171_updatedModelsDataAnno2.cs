namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedModelsDataAnno2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkTasks", "Type", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.WorkTasks", "Type");
        }
    }
}
