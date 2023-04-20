namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remarkclass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Remarks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false),
                        WorkTask_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkTasks", t => t.WorkTask_Id)
                .Index(t => t.WorkTask_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Remarks", "WorkTask_Id", "dbo.WorkTasks");
            DropIndex("dbo.Remarks", new[] { "WorkTask_Id" });
            DropTable("dbo.Remarks");
        }
    }
}
