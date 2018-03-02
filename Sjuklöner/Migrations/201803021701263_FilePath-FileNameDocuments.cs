namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FilePathFileNameDocuments : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Documents", "FilePath", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Documents", "FilePath");
        }
    }
}
