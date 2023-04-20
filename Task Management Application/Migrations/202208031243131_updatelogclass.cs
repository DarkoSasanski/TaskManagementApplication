namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatelogclass : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TaskLogs", "Author_Id", "dbo.AspNetUsers");
            DropIndex("dbo.TaskLogs", new[] { "Author_Id" });
            AddColumn("dbo.TaskLogs", "FirstName", c => c.String());
            AddColumn("dbo.TaskLogs", "LastName", c => c.String());
            AddColumn("dbo.TaskLogs", "Username", c => c.String());
            DropColumn("dbo.TaskLogs", "Author_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TaskLogs", "Author_Id", c => c.String(maxLength: 128));
            DropColumn("dbo.TaskLogs", "Username");
            DropColumn("dbo.TaskLogs", "LastName");
            DropColumn("dbo.TaskLogs", "FirstName");
            CreateIndex("dbo.TaskLogs", "Author_Id");
            AddForeignKey("dbo.TaskLogs", "Author_Id", "dbo.AspNetUsers", "Id");
        }
    }
}
