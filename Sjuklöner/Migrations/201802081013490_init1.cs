namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CareCompanies", "SelectedCollectiveAgreementId", c => c.Int());
            DropColumn("dbo.CareCompanies", "OmbudId");
            DropColumn("dbo.CareCompanies", "CollectiveAgreement");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CareCompanies", "CollectiveAgreement", c => c.String());
            AddColumn("dbo.CareCompanies", "OmbudId", c => c.Int(nullable: false));
            DropColumn("dbo.CareCompanies", "SelectedCollectiveAgreementId");
        }
    }
}
