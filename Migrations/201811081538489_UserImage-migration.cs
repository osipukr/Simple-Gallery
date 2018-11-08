namespace Simply_Gallery.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class UserImagemigration : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Albums", "Profile_Id", "dbo.Profiles");
            DropIndex("dbo.Albums", new[] { "Profile_Id" });
            RenameColumn(table: "dbo.Albums", name: "Profile_Id", newName: "ProfileId");
            AddColumn("dbo.Profiles", "UserName", c => c.String());
            AddColumn("dbo.Profiles", "UserImage", c => c.Binary());
            AlterColumn("dbo.Albums", "ProfileId", c => c.Int(nullable: false));
            CreateIndex("dbo.Albums", "ProfileId");
            AddForeignKey("dbo.Albums", "ProfileId", "dbo.Profiles", "Id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Albums", "ProfileId", "dbo.Profiles");
            DropIndex("dbo.Albums", new[] { "ProfileId" });
            AlterColumn("dbo.Albums", "ProfileId", c => c.Int());
            DropColumn("dbo.Profiles", "UserImage");
            DropColumn("dbo.Profiles", "UserName");
            RenameColumn(table: "dbo.Albums", name: "ProfileId", newName: "Profile_Id");
            CreateIndex("dbo.Albums", "Profile_Id");
            AddForeignKey("dbo.Albums", "Profile_Id", "dbo.Profiles", "Id");
        }
    }
}