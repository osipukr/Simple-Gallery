namespace Simply_Gallery.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Albummigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Albums",
                c => new
                    {
                        AlbumId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Profile_Id = c.Int(),
                    })
                .PrimaryKey(t => t.AlbumId)
                .ForeignKey("dbo.Profiles", t => t.Profile_Id)
                .Index(t => t.Profile_Id);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Pictures",
                c => new
                    {
                        PictureId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Image = c.Binary(),
                        AlbumId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PictureId)
                .ForeignKey("dbo.Albums", t => t.AlbumId, cascadeDelete: true)
                .Index(t => t.AlbumId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Pictures", "AlbumId", "dbo.Albums");
            DropForeignKey("dbo.Albums", "Profile_Id", "dbo.Profiles");
            DropIndex("dbo.Pictures", new[] { "AlbumId" });
            DropIndex("dbo.Albums", new[] { "Profile_Id" });
            DropTable("dbo.Pictures");
            DropTable("dbo.Profiles");
            DropTable("dbo.Albums");
        }
    }
}
