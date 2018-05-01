namespace Sjuklöner.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedDecisionCandidateClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DecisionCandidates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ReferenceNumber = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DecisionCandidates");
        }
    }
}
