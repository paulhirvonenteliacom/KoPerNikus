namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class statusToDocStatus : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Documents", name: "StatusId", newName: "DocStatusId");
            RenameIndex(table: "dbo.Documents", name: "IX_StatusId", newName: "IX_DocStatusId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Documents", name: "IX_DocStatusId", newName: "IX_StatusId");
            RenameColumn(table: "dbo.Documents", name: "DocStatusId", newName: "StatusId");
        }
    }
}
