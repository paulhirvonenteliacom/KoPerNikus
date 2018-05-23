namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedSelecetdSubAssistantIndecesPropertytoClaim : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Claims", "SelectedSubAssistantIndeces", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Claims", "SelectedSubAssistantIndeces");
        }
    }
}
