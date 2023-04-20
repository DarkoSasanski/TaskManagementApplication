namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dataannotaitionsupdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.QuestionAndAnswers", "Question", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.QuestionAndAnswers", "Question", c => c.String(nullable: false));
        }
    }
}
