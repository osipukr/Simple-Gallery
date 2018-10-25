namespace Simply_Gallery.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetRoles", "Description", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetRoles", "Description");
        }
    }
}