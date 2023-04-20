namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingforfeitedandfinishedworkers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FinishedWorkers",
                c => new
                {
                    Id = c.Int(nullable: false, identity: true),
                    Username = c.String(),
                    DateFinished = c.DateTime(nullable: false),
                    WorkTaskId = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkTasks", t => t.WorkTaskId, cascadeDelete: true)
                .Index(t => t.WorkTaskId);
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
            
    
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FinishedWorkers", "WorkTaskId", "dbo.WorkTasks");
            DropIndex("dbo.FinishedWorkers", new[] { "WorkTaskId" });
            DropTable("dbo.FinishedWorkers");
            DropForeignKey("dbo.ForfeitedWorkers", "WorkTaskId", "dbo.WorkTasks");
            DropIndex("dbo.ForfeitedWorkers", new[] { "WorkTaskId" });
            DropTable("dbo.ForfeitedWorkers");
        }
    }
}
