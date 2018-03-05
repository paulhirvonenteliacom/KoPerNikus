namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addReferenceNumberToDocument : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "ReferenceNumber", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Documents", "ReferenceNumber");
        }
    }
}
