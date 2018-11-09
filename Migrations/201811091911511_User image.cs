namespace Simply_Gallery.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Userimage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Image", c => c.Binary());
            AddColumn("dbo.AspNetUsers", "ImageMimeType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "ImageMimeType");
            DropColumn("dbo.AspNetUsers", "Image");
        }
    }
}