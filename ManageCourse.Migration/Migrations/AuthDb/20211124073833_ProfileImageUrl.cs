using Microsoft.EntityFrameworkCore.Migrations;

namespace ManageCourse.Migrations.Migrations.AuthDb
{
    public partial class ProfileImageUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfileImageUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileImageUrl",
                table: "AspNetUsers");
        }
    }
}
