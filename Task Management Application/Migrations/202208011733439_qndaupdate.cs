namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class qndaupdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.QuestionAndAnswers", "WorkTask_Id", "dbo.WorkTasks");
            DropIndex("dbo.QuestionAndAnswers", new[] { "WorkTask_Id" });
            RenameColumn(table: "dbo.QuestionAndAnswers", name: "WorkTask_Id", newName: "WorkTaskId");
            AlterColumn("dbo.QuestionAndAnswers", "WorkTaskId", c => c.Int(nullable: false));
            CreateIndex("dbo.QuestionAndAnswers", "WorkTaskId");
            AddForeignKey("dbo.QuestionAndAnswers", "WorkTaskId", "dbo.WorkTasks", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuestionAndAnswers", "WorkTaskId", "dbo.WorkTasks");
            DropIndex("dbo.QuestionAndAnswers", new[] { "WorkTaskId" });
            AlterColumn("dbo.QuestionAndAnswers", "WorkTaskId", c => c.Int());
            RenameColumn(table: "dbo.QuestionAndAnswers", name: "WorkTaskId", newName: "WorkTask_Id");
            CreateIndex("dbo.QuestionAndAnswers", "WorkTask_Id");
            AddForeignKey("dbo.QuestionAndAnswers", "WorkTask_Id", "dbo.WorkTasks", "Id");
        }
    }
}
