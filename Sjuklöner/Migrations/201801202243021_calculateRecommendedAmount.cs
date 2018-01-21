namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class calculateRecommendedAmount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClaimDays", "Hours", c => c.String());
            AddColumn("dbo.ClaimDays", "UnsocialEvening", c => c.String());
            AddColumn("dbo.ClaimDays", "UnsocialNight", c => c.String());
            AddColumn("dbo.ClaimDays", "UnsocialWeekend", c => c.String());
            AddColumn("dbo.ClaimDays", "UnsocialGrandWeekend", c => c.String());
            AddColumn("dbo.ClaimDays", "OnCallDay", c => c.String());
            AddColumn("dbo.ClaimDays", "OnCallNight", c => c.String());
            AddColumn("dbo.ClaimDays", "HoursSI", c => c.String());
            AddColumn("dbo.ClaimDays", "UnsocialEveningSI", c => c.String());
            AddColumn("dbo.ClaimDays", "UnsocialNightSI", c => c.String());
            AddColumn("dbo.ClaimDays", "UnsocialWeekendSI", c => c.String());
            AddColumn("dbo.ClaimDays", "UnsocialGrandWeekendSI", c => c.String());
            AddColumn("dbo.ClaimDays", "OnCallDaySI", c => c.String());
            AddColumn("dbo.ClaimDays", "OnCallNightSI", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClaimDays", "OnCallNightSI");
            DropColumn("dbo.ClaimDays", "OnCallDaySI");
            DropColumn("dbo.ClaimDays", "UnsocialGrandWeekendSI");
            DropColumn("dbo.ClaimDays", "UnsocialWeekendSI");
            DropColumn("dbo.ClaimDays", "UnsocialNightSI");
            DropColumn("dbo.ClaimDays", "UnsocialEveningSI");
            DropColumn("dbo.ClaimDays", "HoursSI");
            DropColumn("dbo.ClaimDays", "OnCallNight");
            DropColumn("dbo.ClaimDays", "OnCallDay");
            DropColumn("dbo.ClaimDays", "UnsocialGrandWeekend");
            DropColumn("dbo.ClaimDays", "UnsocialWeekend");
            DropColumn("dbo.ClaimDays", "UnsocialNight");
            DropColumn("dbo.ClaimDays", "UnsocialEvening");
            DropColumn("dbo.ClaimDays", "Hours");
        }
    }
}
