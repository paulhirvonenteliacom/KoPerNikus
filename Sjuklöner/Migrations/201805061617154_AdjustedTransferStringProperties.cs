namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdjustedTransferStringProperties : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "QualifyingDateAsString", c => c.String());
            AddColumn("dbo.Claims", "LastDayOfSicknessDateAsString", c => c.String());
            AddColumn("dbo.Claims", "SentInDateAsString", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "SentInDateAsString");
            DropColumn("dbo.Claims", "LastDayOfSicknessDateAsString");
            DropColumn("dbo.Claims", "QualifyingDateAsString");
        }
    }
}
