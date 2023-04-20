namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class statusperworker3 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FinishedWorkers", "WorkTask_Id", "dbo.WorkTasks");
            DropIndex("dbo.FinishedWorkers", new[] { "WorkTask_Id" });
            RenameColumn(table: "dbo.FinishedWorkers", name: "WorkTask_Id", newName: "WorkTaskId");
            AlterColumn("dbo.FinishedWorkers", "WorkTaskId", c => c.Int(nullable: false));
            CreateIndex("dbo.FinishedWorkers", "WorkTaskId");
            AddForeignKey("dbo.FinishedWorkers", "WorkTaskId", "dbo.WorkTasks", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FinishedWorkers", "WorkTaskId", "dbo.WorkTasks");
            DropIndex("dbo.FinishedWorkers", new[] { "WorkTaskId" });
            AlterColumn("dbo.FinishedWorkers", "WorkTaskId", c => c.Int());
            RenameColumn(table: "dbo.FinishedWorkers", name: "WorkTaskId", newName: "WorkTask_Id");
            CreateIndex("dbo.FinishedWorkers", "WorkTask_Id");
            AddForeignKey("dbo.FinishedWorkers", "WorkTask_Id", "dbo.WorkTasks", "Id");
        }
    }
}
