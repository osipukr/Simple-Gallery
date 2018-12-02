namespace Simply_Gallery.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Gallery : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Albums", "Date", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Albums", "Date");
        }
    }
}