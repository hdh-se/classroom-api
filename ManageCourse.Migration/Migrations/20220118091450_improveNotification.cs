using Microsoft.EntityFrameworkCore.Migrations;

namespace ManageCourse.Migrations.Migrations
{
    public partial class improveNotification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Notification",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GradeId",
                table: "Notification",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GradeReview",
                table: "Notification",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "GradeId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "GradeReview",
                table: "Notification");
        }
    }
}
