using Microsoft.EntityFrameworkCore.Migrations;

namespace ManageCourse.Migrations.Migrations
{
    public partial class changeGradeReviewCommentV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewComment_Student_StudentId",
                table: "ReviewComment");

            migrationBuilder.DropIndex(
                name: "IX_ReviewComment_StudentId",
                table: "ReviewComment");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ReviewComment_StudentId",
                table: "ReviewComment",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewComment_Student_StudentId",
                table: "ReviewComment",
                column: "StudentId",
                principalTable: "Student",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
