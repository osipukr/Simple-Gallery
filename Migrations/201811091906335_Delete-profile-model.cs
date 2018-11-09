namespace Simply_Gallery.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Deleteprofilemodel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Albums", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.Albums", new[] { "ProfileId" });
            AddColumn("dbo.Albums", "UserId", c => c.String());
            DropColumn("dbo.Albums", "ProfileId");
            DropTable("dbo.Profiles");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(),
                        UserImage = c.Binary(),
                        ImageMimeType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Albums", "ProfileId", c => c.Int(nullable: false));
            DropColumn("dbo.Albums", "UserId");
            CreateIndex("dbo.Albums", "ProfileId");
            AddForeignKey("dbo.Albums", "ProfileId", "dbo.Profiles", "Id", cascadeDelete: true);
        }
    }
}