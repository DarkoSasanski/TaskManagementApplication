namespace Task_Management_Application.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remarkclass2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Remarks", "Layout", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Remarks", "Layout");
        }
    }
}
