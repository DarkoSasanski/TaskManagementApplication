namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateanswers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.QuestionAndAnswers", "Answering_User_Id", "dbo.AspNetUsers");
            DropIndex("dbo.QuestionAndAnswers", new[] { "Answering_User_Id" });
            CreateTable(
                "dbo.Answers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        QuestionAndAnswerId = c.Int(nullable: false),
                        Text = c.String(),
                        TimeAnswered = c.DateTime(nullable: false),
                        UserAnswered_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserAnswered_Id)
                .ForeignKey("dbo.QuestionAndAnswers", t => t.QuestionAndAnswerId, cascadeDelete: true)
                .Index(t => t.QuestionAndAnswerId)
                .Index(t => t.UserAnswered_Id);
            
            DropColumn("dbo.QuestionAndAnswers", "Answering_Time");
            DropColumn("dbo.QuestionAndAnswers", "Answer");
            DropColumn("dbo.QuestionAndAnswers", "Answering_User_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.QuestionAndAnswers", "Answering_User_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.QuestionAndAnswers", "Answer", c => c.String());
            AddColumn("dbo.QuestionAndAnswers", "Answering_Time", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.Answers", "QuestionAndAnswerId", "dbo.QuestionAndAnswers");
            DropForeignKey("dbo.Answers", "UserAnswered_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Answers", new[] { "UserAnswered_Id" });
            DropIndex("dbo.Answers", new[] { "QuestionAndAnswerId" });
            DropTable("dbo.Answers");
            CreateIndex("dbo.QuestionAndAnswers", "Answering_User_Id");
            AddForeignKey("dbo.QuestionAndAnswers", "Answering_User_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
