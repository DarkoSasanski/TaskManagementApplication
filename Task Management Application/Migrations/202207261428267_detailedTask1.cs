namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class detailedTask1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetUsers", "WorkTask_Id", "dbo.WorkTasks");
            DropForeignKey("dbo.WorkTasks", "Creator_ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.WorkTasks", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetUsers", new[] { "WorkTask_Id" });
            DropIndex("dbo.WorkTasks", new[] { "Creator_ApplicationUser_Id" });
            DropIndex("dbo.WorkTasks", new[] { "ApplicationUser_Id" });
            CreateTable(
                "dbo.WorkTaskApplicationUsers",
                c => new
                    {
                        WorkTask_Id = c.Int(nullable: false),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.WorkTask_Id, t.ApplicationUser_Id })
                .ForeignKey("dbo.WorkTasks", t => t.WorkTask_Id, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id, cascadeDelete: true)
                .Index(t => t.WorkTask_Id)
                .Index(t => t.ApplicationUser_Id);
            
            DropColumn("dbo.AspNetUsers", "WorkTask_Id");
            DropColumn("dbo.WorkTasks", "Creator_ApplicationUser_Id");
            DropColumn("dbo.WorkTasks", "ApplicationUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.WorkTasks", "ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.WorkTasks", "Creator_ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUsers", "WorkTask_Id", c => c.Int());
            DropForeignKey("dbo.WorkTaskApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.WorkTaskApplicationUsers", "WorkTask_Id", "dbo.WorkTasks");
            DropIndex("dbo.WorkTaskApplicationUsers", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.WorkTaskApplicationUsers", new[] { "WorkTask_Id" });
            DropTable("dbo.WorkTaskApplicationUsers");
            CreateIndex("dbo.WorkTasks", "ApplicationUser_Id");
            CreateIndex("dbo.WorkTasks", "Creator_ApplicationUser_Id");
            CreateIndex("dbo.AspNetUsers", "WorkTask_Id");
            AddForeignKey("dbo.WorkTasks", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.WorkTasks", "Creator_ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetUsers", "WorkTask_Id", "dbo.WorkTasks", "Id");
        }
    }
}
