namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatenameuser : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Answers", "UserAnswered_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.QuestionAndAnswers", "Asking_User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.QuestionAndAnswers", new[] { "Asking_User_Id" });
            DropIndex("dbo.Answers", new[] { "UserAnswered_Id" });
            AddColumn("dbo.QuestionAndAnswers", "FirstName", c => c.String());
            AddColumn("dbo.QuestionAndAnswers", "LastName", c => c.String());
            AddColumn("dbo.Answers", "FirstName", c => c.String());
            AddColumn("dbo.Answers", "LastName", c => c.String());
            DropColumn("dbo.QuestionAndAnswers", "Asking_User_Id");
            DropColumn("dbo.Answers", "UserAnswered_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Answers", "UserAnswered_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.QuestionAndAnswers", "Asking_User_Id", c => c.String(maxLength: 128));
            DropColumn("dbo.Answers", "LastName");
            DropColumn("dbo.Answers", "FirstName");
            DropColumn("dbo.QuestionAndAnswers", "LastName");
            DropColumn("dbo.QuestionAndAnswers", "FirstName");
            CreateIndex("dbo.Answers", "UserAnswered_Id");
            CreateIndex("dbo.QuestionAndAnswers", "Asking_User_Id");
            AddForeignKey("dbo.QuestionAndAnswers", "Asking_User_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.Answers", "UserAnswered_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
