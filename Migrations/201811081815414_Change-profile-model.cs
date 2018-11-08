namespace Simply_Gallery.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Changeprofilemodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Profiles", "ImageMimeType", c => c.String());
            DropColumn("dbo.Profiles", "UserName");
            DropColumn("dbo.Pictures", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Pictures", "Name", c => c.String());
            AddColumn("dbo.Profiles", "UserName", c => c.String());
            DropColumn("dbo.Profiles", "ImageMimeType");
        }
    }
}
