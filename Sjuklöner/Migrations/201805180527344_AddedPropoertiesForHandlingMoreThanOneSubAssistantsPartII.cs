namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedPropoertiesForHandlingMoreThanOneSubAssistantsPartII : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "SubAssistantsNameConcat", c => c.String());
            AddColumn("dbo.Claims", "SubAssistantsSSNConcat", c => c.String());
            AddColumn("dbo.Claims", "SubAssistantsEmailConcat", c => c.String());
            AddColumn("dbo.Claims", "SubAssistantsPhoneConcat", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "SubAssistantsPhoneConcat");
            DropColumn("dbo.Claims", "SubAssistantsEmailConcat");
            DropColumn("dbo.Claims", "SubAssistantsSSNConcat");
            DropColumn("dbo.Claims", "SubAssistantsNameConcat");
        }
    }
}
