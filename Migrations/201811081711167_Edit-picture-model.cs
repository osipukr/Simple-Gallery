namespace Simply_Gallery.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Editpicturemodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Pictures", "Name", c => c.String());
            AddColumn("dbo.Pictures", "ImageMimeType", c => c.String());
            DropColumn("dbo.Pictures", "Description");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Pictures", "Description", c => c.String());
            DropColumn("dbo.Pictures", "ImageMimeType");
            DropColumn("dbo.Pictures", "Name");
        }
    }
}