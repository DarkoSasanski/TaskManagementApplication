namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatedModelsDataAnno : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.WorkTasks", "Status", c => c.String());
            AddColumn("dbo.WorkTasks", "CreatorUserName", c => c.String());
            AlterColumn("dbo.QuestionAndAnswers", "Question", c => c.String(nullable: false));
            AlterColumn("dbo.WorkTasks", "Title", c => c.String(nullable: false));
            AlterColumn("dbo.TaskLogs", "Title", c => c.String(nullable: false));
            AlterColumn("dbo.TaskLogs", "Text", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TaskLogs", "Text", c => c.String());
            AlterColumn("dbo.TaskLogs", "Title", c => c.String());
            AlterColumn("dbo.WorkTasks", "Title", c => c.String());
            AlterColumn("dbo.QuestionAndAnswers", "Question", c => c.String());
            DropColumn("dbo.WorkTasks", "CreatorUserName");
            DropColumn("dbo.WorkTasks", "Status");
        }
    }
}
