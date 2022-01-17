using Microsoft.EntityFrameworkCore.Migrations;

namespace ManageCourse.Migrations.Migrations
{
    public partial class changeGradeReviewComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewComment_Student_StudentId",
                table: "ReviewComment");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TeacherNotification",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "StudentNotification",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewComment_Student_StudentId",
                table: "ReviewComment",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewComment_Student_StudentId",
                table: "ReviewComment");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TeacherNotification");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StudentNotification");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewComment_Student_StudentId",
                table: "ReviewComment",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
