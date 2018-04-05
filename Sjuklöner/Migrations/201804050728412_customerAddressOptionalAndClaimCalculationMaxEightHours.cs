namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class customerAddressOptionalAndClaimCalculationMaxEightHours : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClaimCalculations", "PaidHoursQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "SalaryQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "SalaryCalcQD", c => c.String());
            AlterColumn("dbo.Claims", "CustomerAddress", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Claims", "CustomerAddress", c => c.String(nullable: false));
            DropColumn("dbo.ClaimCalculations", "SalaryCalcQD");
            DropColumn("dbo.ClaimCalculations", "SalaryQD");
            DropColumn("dbo.ClaimCalculations", "PaidHoursQD");
        }
    }
}
