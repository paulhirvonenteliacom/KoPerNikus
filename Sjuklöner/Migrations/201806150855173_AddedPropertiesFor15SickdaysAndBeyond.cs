namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPropertiesFor15SickdaysAndBeyond : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClaimCalculations", "HoursD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "SalaryD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "SalaryCalcD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "SickPayD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "SickPayCalcD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "HolidayPayD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "HolidayPayCalcD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialEveningD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialEveningPayD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialEveningPayCalcD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialNightD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialNightPayD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialNightPayCalcD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialWeekendD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialWeekendPayD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialWeekendPayCalcD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialGrandWeekendD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialGrandWeekendPayD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialGrandWeekendPayCalcD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialSumD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialSumPayD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "UnsocialSumPayCalcD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "OnCallDayD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "OnCallDayPayD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "OnCallDayPayCalcD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "OnCallNightD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "OnCallNightPayD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "OnCallNightPayCalcD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "OnCallSumD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "OnCallSumPayD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "OnCallSumPayCalcD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "SocialFeesD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "SocialFeesCalcD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "PensionAndInsuranceD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "PensionAndInsuranceCalcD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "CostD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "CostCalcD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "TotalCostD1Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "TotalCostCalcD1Plus", c => c.String());
            AddColumn("dbo.ClaimDays", "CalendarDayNumber", c => c.Int(nullable: false));
            AddColumn("dbo.Claims", "NumberOfCalendarDays", c => c.Int(nullable: false));
            AddColumn("dbo.Claims", "TotalCostD1Plus", c => c.String());
            AddColumn("dbo.Claims", "TotalCostCalcD1Plus", c => c.String());
            AlterColumn("dbo.ClaimDays", "SickDayNumber", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ClaimDays", "SickDayNumber", c => c.Int(nullable: false));
            DropColumn("dbo.Claims", "TotalCostCalcD1Plus");
            DropColumn("dbo.Claims", "TotalCostD1Plus");
            DropColumn("dbo.Claims", "NumberOfCalendarDays");
            DropColumn("dbo.ClaimDays", "CalendarDayNumber");
            DropColumn("dbo.ClaimCalculations", "TotalCostCalcD1Plus");
            DropColumn("dbo.ClaimCalculations", "TotalCostD1Plus");
            DropColumn("dbo.ClaimCalculations", "CostCalcD15Plus");
            DropColumn("dbo.ClaimCalculations", "CostD15Plus");
            DropColumn("dbo.ClaimCalculations", "PensionAndInsuranceCalcD15Plus");
            DropColumn("dbo.ClaimCalculations", "PensionAndInsuranceD15Plus");
            DropColumn("dbo.ClaimCalculations", "SocialFeesCalcD15Plus");
            DropColumn("dbo.ClaimCalculations", "SocialFeesD15Plus");
            DropColumn("dbo.ClaimCalculations", "OnCallSumPayCalcD15Plus");
            DropColumn("dbo.ClaimCalculations", "OnCallSumPayD15Plus");
            DropColumn("dbo.ClaimCalculations", "OnCallSumD15Plus");
            DropColumn("dbo.ClaimCalculations", "OnCallNightPayCalcD15Plus");
            DropColumn("dbo.ClaimCalculations", "OnCallNightPayD15Plus");
            DropColumn("dbo.ClaimCalculations", "OnCallNightD15Plus");
            DropColumn("dbo.ClaimCalculations", "OnCallDayPayCalcD15Plus");
            DropColumn("dbo.ClaimCalculations", "OnCallDayPayD15Plus");
            DropColumn("dbo.ClaimCalculations", "OnCallDayD15Plus");
            DropColumn("dbo.ClaimCalculations", "UnsocialSumPayCalcD15Plus");
            DropColumn("dbo.ClaimCalculations", "UnsocialSumPayD15Plus");
            DropColumn("dbo.ClaimCalculations", "UnsocialSumD15Plus");
            DropColumn("dbo.ClaimCalculations", "UnsocialGrandWeekendPayCalcD15Plus");
            DropColumn("dbo.ClaimCalculations", "UnsocialGrandWeekendPayD15Plus");
            DropColumn("dbo.ClaimCalculations", "UnsocialGrandWeekendD15Plus");
            DropColumn("dbo.ClaimCalculations", "UnsocialWeekendPayCalcD15Plus");
            DropColumn("dbo.ClaimCalculations", "UnsocialWeekendPayD15Plus");
            DropColumn("dbo.ClaimCalculations", "UnsocialWeekendD15Plus");
            DropColumn("dbo.ClaimCalculations", "UnsocialNightPayCalcD15Plus");
            DropColumn("dbo.ClaimCalculations", "UnsocialNightPayD15Plus");
            DropColumn("dbo.ClaimCalculations", "UnsocialNightD15Plus");
            DropColumn("dbo.ClaimCalculations", "UnsocialEveningPayCalcD15Plus");
            DropColumn("dbo.ClaimCalculations", "UnsocialEveningPayD15Plus");
            DropColumn("dbo.ClaimCalculations", "UnsocialEveningD15Plus");
            DropColumn("dbo.ClaimCalculations", "HolidayPayCalcD15Plus");
            DropColumn("dbo.ClaimCalculations", "HolidayPayD15Plus");
            DropColumn("dbo.ClaimCalculations", "SickPayCalcD15Plus");
            DropColumn("dbo.ClaimCalculations", "SickPayD15Plus");
            DropColumn("dbo.ClaimCalculations", "SalaryCalcD15Plus");
            DropColumn("dbo.ClaimCalculations", "SalaryD15Plus");
            DropColumn("dbo.ClaimCalculations", "HoursD15Plus");
        }
    }
}
