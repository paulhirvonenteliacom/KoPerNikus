namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMissingDoctorsCertificateProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "MissingDoctorsCertificate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "MissingDoctorsCertificate");
        }
    }
}
