namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedModelsDataAnno1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.WorkTasks", "Description", c => c.String(nullable: false));
            DropColumn("dbo.WorkTasks", "Add_Remarks");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WorkTasks", "Add_Remarks", c => c.String());
            AlterColumn("dbo.WorkTasks", "Description", c => c.String());
        }
    }
}
