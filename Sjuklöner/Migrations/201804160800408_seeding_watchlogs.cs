namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class seeding_watchlogs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.WatchLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LogCode = c.Int(nullable: false),
                        LogDate = c.DateTime(nullable: false),
                        Robot = c.String(),
                        LogMsg = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.WatchLogs");
        }
    }
}
