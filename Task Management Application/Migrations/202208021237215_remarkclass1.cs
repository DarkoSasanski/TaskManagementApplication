namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remarkclass1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Remarks", "WorkTask_Id", "dbo.WorkTasks");
            DropIndex("dbo.Remarks", new[] { "WorkTask_Id" });
            RenameColumn(table: "dbo.Remarks", name: "WorkTask_Id", newName: "WorkTaskId");
            AlterColumn("dbo.Remarks", "WorkTaskId", c => c.Int(nullable: false));
            CreateIndex("dbo.Remarks", "WorkTaskId");
            AddForeignKey("dbo.Remarks", "WorkTaskId", "dbo.WorkTasks", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Remarks", "WorkTaskId", "dbo.WorkTasks");
            DropIndex("dbo.Remarks", new[] { "WorkTaskId" });
            AlterColumn("dbo.Remarks", "WorkTaskId", c => c.Int());
            RenameColumn(table: "dbo.Remarks", name: "WorkTaskId", newName: "WorkTask_Id");
            CreateIndex("dbo.Remarks", "WorkTask_Id");
            AddForeignKey("dbo.Remarks", "WorkTask_Id", "dbo.WorkTasks", "Id");
        }
    }
}
