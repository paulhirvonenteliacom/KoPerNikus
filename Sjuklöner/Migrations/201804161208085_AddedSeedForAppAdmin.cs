namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSeedForAppAdmin : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AppAdmins",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AutomaticTransferToProcapita = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AppAdmins");
        }
    }
}
