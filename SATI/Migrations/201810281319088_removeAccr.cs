namespace SATI.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class removeAccr : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "Accredited");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Accredited", c => c.Boolean(nullable: false));
        }
    }
}
