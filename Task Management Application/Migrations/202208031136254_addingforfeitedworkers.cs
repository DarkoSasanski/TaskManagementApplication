namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingforfeitedworkers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.FinishedWorkers", "WorkTask_Id", "dbo.WorkTasks");
            DropForeignKey("dbo.FinishedWorkers", "WorkTask_Id1", "dbo.WorkTasks");
            DropIndex("dbo.FinishedWorkers", new[] { "WorkTask_Id" });
            DropIndex("dbo.FinishedWorkers", new[] { "WorkTask_Id1" });
            DropColumn("dbo.FinishedWorkers", "WorkTaskId");
            RenameColumn(table: "dbo.FinishedWorkers", name: "WorkTask_Id", newName: "WorkTaskId");
            RenameColumn(table: "dbo.ForfeitedWorkers", name: "WorkTask_Id1", newName: "WorkTaskId");
            CreateTable(
                "dbo.ForfeitedWorkers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WorkTaskId = c.Int(nullable: false),
                        Username = c.String(),
                        DateFinished = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkTasks", t => t.WorkTaskId, cascadeDelete: true)
                .Index(t => t.WorkTaskId);
            
            AlterColumn("dbo.FinishedWorkers", "WorkTaskId", c => c.Int(nullable: false));
            CreateIndex("dbo.FinishedWorkers", "WorkTaskId");
            AddForeignKey("dbo.FinishedWorkers", "WorkTaskId", "dbo.WorkTasks", "Id", cascadeDelete: true);
            DropColumn("dbo.FinishedWorkers", "WorkTask_Id1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.FinishedWorkers", "WorkTask_Id1", c => c.Int());
            DropForeignKey("dbo.ForfeitedWorkers", "WorkTaskId", "dbo.WorkTasks");
            DropForeignKey("dbo.FinishedWorkers", "WorkTaskId", "dbo.WorkTasks");
            DropIndex("dbo.ForfeitedWorkers", new[] { "WorkTaskId" });
            DropIndex("dbo.FinishedWorkers", new[] { "WorkTaskId" });
            AlterColumn("dbo.FinishedWorkers", "WorkTaskId", c => c.Int());
            DropTable("dbo.ForfeitedWorkers");
            RenameColumn(table: "dbo.ForfeitedWorkers", name: "WorkTaskId", newName: "WorkTask_Id1");
            RenameColumn(table: "dbo.FinishedWorkers", name: "WorkTaskId", newName: "WorkTask_Id");
            AddColumn("dbo.FinishedWorkers", "WorkTaskId", c => c.Int(nullable: false));
            CreateIndex("dbo.FinishedWorkers", "WorkTask_Id1");
            CreateIndex("dbo.FinishedWorkers", "WorkTask_Id");
            AddForeignKey("dbo.FinishedWorkers", "WorkTask_Id1", "dbo.WorkTasks", "Id");
            AddForeignKey("dbo.FinishedWorkers", "WorkTask_Id", "dbo.WorkTasks", "Id");
        }
    }
}
