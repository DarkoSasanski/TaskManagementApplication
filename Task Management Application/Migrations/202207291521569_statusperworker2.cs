namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class statusperworker2 : DbMigration
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
                        WorkTask_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkTasks", t => t.WorkTask_Id)
                .Index(t => t.WorkTask_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FinishedWorkers", "WorkTask_Id", "dbo.WorkTasks");
            DropIndex("dbo.FinishedWorkers", new[] { "WorkTask_Id" });
            DropTable("dbo.FinishedWorkers");
        }
    }
}
