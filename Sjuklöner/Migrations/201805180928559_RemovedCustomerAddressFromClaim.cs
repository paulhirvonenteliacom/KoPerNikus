namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemovedCustomerAddressFromClaim : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Claims", "CustomerAddress");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Claims", "CustomerAddress", c => c.String());
        }
    }
}
