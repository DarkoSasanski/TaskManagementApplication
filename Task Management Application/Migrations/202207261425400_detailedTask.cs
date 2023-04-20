namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class detailedTask : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.WorkTaskApplicationUsers", "WorkTask_Id", "dbo.WorkTasks");
            DropForeignKey("dbo.WorkTaskApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.WorkTaskApplicationUsers", new[] { "WorkTask_Id" });
            DropIndex("dbo.WorkTaskApplicationUsers", new[] { "ApplicationUser_Id" });
            CreateTable(
                "dbo.QuestionAndAnswers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Asking_Time = c.DateTime(nullable: false),
                        Question = c.String(),
                        Answering_Time = c.DateTime(nullable: false),
                        Answer = c.String(),
                        WorkTask_Id = c.Int(),
                        Answering_User_Id = c.String(maxLength: 128),
                        Asking_User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.WorkTasks", t => t.WorkTask_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Answering_User_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Asking_User_Id)
                .Index(t => t.WorkTask_Id)
                .Index(t => t.Answering_User_Id)
                .Index(t => t.Asking_User_Id);
            
            CreateTable(
                "dbo.TaskLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Time_Created = c.DateTime(nullable: false),
                        Title = c.String(),
                        Text = c.String(),
                        Author_Id = c.String(maxLength: 128),
                        WorkTask_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Author_Id)
                .ForeignKey("dbo.WorkTasks", t => t.WorkTask_Id)
                .Index(t => t.Author_Id)
                .Index(t => t.WorkTask_Id);
            
            AddColumn("dbo.AspNetUsers", "WorkTask_Id", c => c.Int());
            AddColumn("dbo.WorkTasks", "Title", c => c.String());
            AddColumn("dbo.WorkTasks", "Description", c => c.String());
            AddColumn("dbo.WorkTasks", "Add_Remarks", c => c.String());
            AddColumn("dbo.WorkTasks", "Deadline", c => c.DateTime(nullable: false));
            AddColumn("dbo.WorkTasks", "Creator_ApplicationUser_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.WorkTasks", "ApplicationUser_Id", c => c.String(maxLength: 128));
            CreateIndex("dbo.AspNetUsers", "WorkTask_Id");
            CreateIndex("dbo.WorkTasks", "Creator_ApplicationUser_Id");
            CreateIndex("dbo.WorkTasks", "ApplicationUser_Id");
            AddForeignKey("dbo.AspNetUsers", "WorkTask_Id", "dbo.WorkTasks", "Id");
            AddForeignKey("dbo.WorkTasks", "Creator_ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.WorkTasks", "ApplicationUser_Id", "dbo.AspNetUsers", "Id");
            DropTable("dbo.WorkTaskApplicationUsers");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.WorkTaskApplicationUsers",
                c => new
                    {
                        WorkTask_Id = c.Int(nullable: false),
                        ApplicationUser_Id = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.WorkTask_Id, t.ApplicationUser_Id });
            
            DropForeignKey("dbo.QuestionAndAnswers", "Asking_User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.QuestionAndAnswers", "Answering_User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.WorkTasks", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.TaskLogs", "WorkTask_Id", "dbo.WorkTasks");
            DropForeignKey("dbo.TaskLogs", "Author_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.QuestionAndAnswers", "WorkTask_Id", "dbo.WorkTasks");
            DropForeignKey("dbo.WorkTasks", "Creator_ApplicationUser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUsers", "WorkTask_Id", "dbo.WorkTasks");
            DropIndex("dbo.TaskLogs", new[] { "WorkTask_Id" });
            DropIndex("dbo.TaskLogs", new[] { "Author_Id" });
            DropIndex("dbo.WorkTasks", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.WorkTasks", new[] { "Creator_ApplicationUser_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "WorkTask_Id" });
            DropIndex("dbo.QuestionAndAnswers", new[] { "Asking_User_Id" });
            DropIndex("dbo.QuestionAndAnswers", new[] { "Answering_User_Id" });
            DropIndex("dbo.QuestionAndAnswers", new[] { "WorkTask_Id" });
            DropColumn("dbo.WorkTasks", "ApplicationUser_Id");
            DropColumn("dbo.WorkTasks", "Creator_ApplicationUser_Id");
            DropColumn("dbo.WorkTasks", "Deadline");
            DropColumn("dbo.WorkTasks", "Add_Remarks");
            DropColumn("dbo.WorkTasks", "Description");
            DropColumn("dbo.WorkTasks", "Title");
            DropColumn("dbo.AspNetUsers", "WorkTask_Id");
            DropTable("dbo.TaskLogs");
            DropTable("dbo.QuestionAndAnswers");
            CreateIndex("dbo.WorkTaskApplicationUsers", "ApplicationUser_Id");
            CreateIndex("dbo.WorkTaskApplicationUsers", "WorkTask_Id");
            AddForeignKey("dbo.WorkTaskApplicationUsers", "ApplicationUser_Id", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.WorkTaskApplicationUsers", "WorkTask_Id", "dbo.WorkTasks", "Id", cascadeDelete: true);
        }
    }
}
