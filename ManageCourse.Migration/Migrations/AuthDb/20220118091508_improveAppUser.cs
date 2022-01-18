using Microsoft.EntityFrameworkCore.Migrations;

namespace ManageCourse.Migrations.Migrations.AuthDb
{
    public partial class improveAppUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleAccount",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RoleAccount",
                table: "AspNetUsers");
        }
    }
}
