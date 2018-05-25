namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class testII : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "NumberOfSubAssistants", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "NumberOfSubAssistants");
        }
    }
}
