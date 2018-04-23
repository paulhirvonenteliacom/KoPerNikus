namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPropertiesForQualifyingDayInClaimCalculationClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClaimCalculations", "UnsocialEveningHoursQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialNightHoursQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialWeekendHoursQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialGrandWeekendHoursQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "PaidUnsocialEveningHoursQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "PaidUnsocialNightHoursQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "PaidUnsocialWeekendHoursQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "PaidUnsocialGrandWeekendHoursQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialEveningPayQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialEveningPayCalcQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialNightPayQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialNightPayCalcQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialWeekendPayQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialWeekendPayCalcQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialGrandWeekendPayQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialGrandWeekendPayCalcQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialSumPayQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialSumPayCalcQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "OnCallDayPayQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "OnCallDayPayCalcQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "OnCallNightPayQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "OnCallNightPayCalcQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "OnCallSumPayQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "OnCallSumPayCalcQD", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClaimCalculations", "OnCallSumPayCalcQD");
            DropColumn("dbo.ClaimCalculations", "OnCallSumPayQD");
            DropColumn("dbo.ClaimCalculations", "OnCallNightPayCalcQD");
            DropColumn("dbo.ClaimCalculations", "OnCallNightPayQD");
            DropColumn("dbo.ClaimCalculations", "OnCallDayPayCalcQD");
            DropColumn("dbo.ClaimCalculations", "OnCallDayPayQD");
            DropColumn("dbo.ClaimCalculations", "UnsocialSumPayCalcQD");
            DropColumn("dbo.ClaimCalculations", "UnsocialSumPayQD");
            DropColumn("dbo.ClaimCalculations", "UnsocialGrandWeekendPayCalcQD");
            DropColumn("dbo.ClaimCalculations", "UnsocialGrandWeekendPayQD");
            DropColumn("dbo.ClaimCalculations", "UnsocialWeekendPayCalcQD");
            DropColumn("dbo.ClaimCalculations", "UnsocialWeekendPayQD");
            DropColumn("dbo.ClaimCalculations", "UnsocialNightPayCalcQD");
            DropColumn("dbo.ClaimCalculations", "UnsocialNightPayQD");
            DropColumn("dbo.ClaimCalculations", "UnsocialEveningPayCalcQD");
            DropColumn("dbo.ClaimCalculations", "UnsocialEveningPayQD");
            DropColumn("dbo.ClaimCalculations", "PaidUnsocialGrandWeekendHoursQD");
            DropColumn("dbo.ClaimCalculations", "PaidUnsocialWeekendHoursQD");
            DropColumn("dbo.ClaimCalculations", "PaidUnsocialNightHoursQD");
            DropColumn("dbo.ClaimCalculations", "PaidUnsocialEveningHoursQD");
            DropColumn("dbo.ClaimCalculations", "UnsocialGrandWeekendHoursQD");
            DropColumn("dbo.ClaimCalculations", "UnsocialWeekendHoursQD");
            DropColumn("dbo.ClaimCalculations", "UnsocialNightHoursQD");
            DropColumn("dbo.ClaimCalculations", "UnsocialEveningHoursQD");
        }
    }
}
