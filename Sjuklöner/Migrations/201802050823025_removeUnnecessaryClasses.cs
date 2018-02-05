namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeUnnecessaryClasses : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ClaimCalculations", "CareCompany_Id", "dbo.CareCompanies");
            DropForeignKey("dbo.Documents", "ClaimCalculation_Id", "dbo.ClaimCalculations");
            DropForeignKey("dbo.Messages", "ClaimCalculation_Id", "dbo.ClaimCalculations");
            DropIndex("dbo.ClaimCalculations", new[] { "CareCompany_Id" });
            DropIndex("dbo.Documents", new[] { "ClaimCalculation_Id" });
            DropIndex("dbo.Messages", new[] { "ClaimCalculation_Id" });
            DropColumn("dbo.ClaimCalculations", "CareCompany_Id");
            DropColumn("dbo.Documents", "ClaimCalculation_Id");
            DropColumn("dbo.Messages", "ClaimCalculation_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Messages", "ClaimCalculation_Id", c => c.Int());
            AddColumn("dbo.Documents", "ClaimCalculation_Id", c => c.Int());
            AddColumn("dbo.ClaimCalculations", "CareCompany_Id", c => c.Int());
            CreateIndex("dbo.Messages", "ClaimCalculation_Id");
            CreateIndex("dbo.Documents", "ClaimCalculation_Id");
            CreateIndex("dbo.ClaimCalculations", "CareCompany_Id");
            AddForeignKey("dbo.Messages", "ClaimCalculation_Id", "dbo.ClaimCalculations", "Id");
            AddForeignKey("dbo.Documents", "ClaimCalculation_Id", "dbo.ClaimCalculations", "Id");
            AddForeignKey("dbo.ClaimCalculations", "CareCompany_Id", "dbo.CareCompanies", "Id");
        }
    }
}
