namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSalaryBasePropertiesToClaimCalculationClass : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClaimCalculations", "SalaryBaseQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "SalaryBaseCalcQD", c => c.String());
            AddColumn("dbo.ClaimCalculations", "SalaryBaseD2T14", c => c.String());
            AddColumn("dbo.ClaimCalculations", "SalaryBaseCalcD2T14", c => c.String());
            AddColumn("dbo.ClaimCalculations", "SalaryBaseD15Plus", c => c.String());
            AddColumn("dbo.ClaimCalculations", "SalaryBaseCalcD15Plus", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ClaimCalculations", "SalaryBaseCalcD15Plus");
            DropColumn("dbo.ClaimCalculations", "SalaryBaseD15Plus");
            DropColumn("dbo.ClaimCalculations", "SalaryBaseCalcD2T14");
            DropColumn("dbo.ClaimCalculations", "SalaryBaseD2T14");
            DropColumn("dbo.ClaimCalculations", "SalaryBaseCalcQD");
            DropColumn("dbo.ClaimCalculations", "SalaryBaseQD");
        }
    }
}
