namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMorePropertiesForQualifyingDayToClaimCalculationClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClaimCalculations", "SickPayQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "SickPayCalcQD", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClaimCalculations", "SickPayCalcQD");
            DropColumn("dbo.ClaimCalculations", "SickPayQD");
        }
    }
}
