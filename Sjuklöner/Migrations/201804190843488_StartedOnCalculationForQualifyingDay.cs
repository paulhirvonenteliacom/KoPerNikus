namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class StartedOnCalculationForQualifyingDay : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClaimCalculations", "OnCallDayHoursQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "OnCallNightHoursQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "PaidOnCallDayHoursQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "PaidOnCallNightHoursQD", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClaimCalculations", "PaidOnCallNightHoursQD");
            DropColumn("dbo.ClaimCalculations", "PaidOnCallDayHoursQD");
            DropColumn("dbo.ClaimCalculations", "OnCallNightHoursQD");
            DropColumn("dbo.ClaimCalculations", "OnCallDayHoursQD");
        }
    }
}
