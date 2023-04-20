namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedModelsDataAnno3 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WorkTasks", "Type", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.WorkTasks", "Type", c => c.String(nullable: false));
        }
    }
}
