namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AdmOffClaims : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "AdmOffId", c => c.String());
            AddColumn("dbo.Claims", "AdmOffName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "AdmOffName");
            DropColumn("dbo.Claims", "AdmOffId");
        }
    }
}
