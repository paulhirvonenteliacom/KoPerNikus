namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedReducedModelSumPropertyMissingDoctorsCertificateToClaim : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "ModelSumReducedDueToMissingDoctorsCertificate", c => c.Boolean(nullable: false));
            AddColumn("dbo.Claims", "Day7OfSicknessDate", c => c.DateTime());
            AddColumn("dbo.Claims", "Day7OfSicknessDateAsString", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "Day7OfSicknessDateAsString");
            DropColumn("dbo.Claims", "Day7OfSicknessDate");
            DropColumn("dbo.Claims", "ModelSumReducedDueToMissingDoctorsCertificate");
        }
    }
}
