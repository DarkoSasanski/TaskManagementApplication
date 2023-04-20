namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedWorkedTasksAndRels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WorkTasks",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Creator_User_Id = c.String(maxLength: 128),
                        ApplicationUser_Id = c.String(maxLength: 128),
                        ApplicationUser_Id1 = c.String(maxLength: 128),
                        ApplicationUser_Id2 = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Creator_User_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id1)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id2)
                .Index(t => t.Creator_User_Id)
                .Index(t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id1)
                .Index(t => t.ApplicationUser_Id2);
            
            AddColumn("dbo.AspNetUsers", "WorkTask_Id", c => c.Int());
            AddColumn("dbo.AspNetUsers", "WorkTask_Id1", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "WorkTask_Id");
            CreateIndex("dbo.AspNetUsers", "WorkTask_Id1");
            AddForeignKey("dbo.AspNetUsers", "WorkTask_Id", "dbo.WorkTasks", "Id");
            AddForeignKey("dbo.AspNetUsers", "WorkTask_Id1", "dbo.WorkTasks", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WorkTasks", "ApplicationUser_Id2", "dbo.AspNetUsers");
            DropForeignKey("dbo.WorkTasks", "ApplicationUser_Id1", "dbo.AspNetUsers");
            DropForeignKey("dbo.WorkTasks", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.WorkTasks", "Creator_User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "WorkTask_Id1", "dbo.WorkTasks");
            DropForeignKey("dbo.AspNetUsers", "WorkTask_Id", "dbo.WorkTasks");
            DropIndex("dbo.WorkTasks", new[] { "ApplicationUser_Id2" });
            DropIndex("dbo.WorkTasks", new[] { "ApplicationUser_Id1" });
            DropIndex("dbo.WorkTasks", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.WorkTasks", new[] { "Creator_User_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "WorkTask_Id1" });
            DropIndex("dbo.AspNetUsers", new[] { "WorkTask_Id" });
            DropColumn("dbo.AspNetUsers", "WorkTask_Id1");
            DropColumn("dbo.AspNetUsers", "WorkTask_Id");
            DropTable("dbo.WorkTasks");
        }
    }
}
