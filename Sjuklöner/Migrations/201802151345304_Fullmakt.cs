namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Fullmakt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "ProxyCheck", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "ProxyCheck");
        }
    }
}
