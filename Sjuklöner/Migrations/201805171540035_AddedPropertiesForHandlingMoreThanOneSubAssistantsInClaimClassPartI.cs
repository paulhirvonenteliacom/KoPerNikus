namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPropertiesForHandlingMoreThanOneSubAssistantsInClaimClassPartI : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "NumberOfHoursWithSIConcat", c => c.String());
            AddColumn("dbo.Claims", "NumberOfOrdinaryHoursSIConcat", c => c.String());
            AddColumn("dbo.Claims", "NumberOfUnsocialHoursSIConcat", c => c.String());
            AddColumn("dbo.Claims", "NumberOfOnCallHoursSIConcat", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "NumberOfOnCallHoursSIConcat");
            DropColumn("dbo.Claims", "NumberOfUnsocialHoursSIConcat");
            DropColumn("dbo.Claims", "NumberOfOrdinaryHoursSIConcat");
            DropColumn("dbo.Claims", "NumberOfHoursWithSIConcat");
        }
    }
}
