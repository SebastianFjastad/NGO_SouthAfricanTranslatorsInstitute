namespace SATI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsAccredited : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Capability", "IsAccredited", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Capability", "IsAccredited");
        }
    }
}
