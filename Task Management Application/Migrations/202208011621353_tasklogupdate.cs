namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tasklogupdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TaskLogs", "WorkTask_Id", "dbo.WorkTasks");
            DropIndex("dbo.TaskLogs", new[] { "WorkTask_Id" });
            RenameColumn(table: "dbo.TaskLogs", name: "WorkTask_Id", newName: "WorkTaskId");
            AlterColumn("dbo.TaskLogs", "WorkTaskId", c => c.Int(nullable: false));
            CreateIndex("dbo.TaskLogs", "WorkTaskId");
            AddForeignKey("dbo.TaskLogs", "WorkTaskId", "dbo.WorkTasks", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TaskLogs", "WorkTaskId", "dbo.WorkTasks");
            DropIndex("dbo.TaskLogs", new[] { "WorkTaskId" });
            AlterColumn("dbo.TaskLogs", "WorkTaskId", c => c.Int());
            RenameColumn(table: "dbo.TaskLogs", name: "WorkTaskId", newName: "WorkTask_Id");
            CreateIndex("dbo.TaskLogs", "WorkTask_Id");
            AddForeignKey("dbo.TaskLogs", "WorkTask_Id", "dbo.WorkTasks", "Id");
        }
    }
}
