namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPropertiesInClaimForMultipleFKAttachments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "FKSubAssistantCheckBoolConcat", c => c.String());
            AddColumn("dbo.Claims", "FKSubAssistantCheckMsgConcat", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "FKSubAssistantCheckMsgConcat");
            DropColumn("dbo.Claims", "FKSubAssistantCheckBoolConcat");
        }
    }
}
