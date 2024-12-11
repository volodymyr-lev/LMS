using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class AddAssignmentEntityAndSyllabus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Syllabus",
                table: "Courses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "CourseId1",
                table: "Assignments",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_CourseId1",
                table: "Assignments",
                column: "CourseId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_Courses_CourseId1",
                table: "Assignments",
                column: "CourseId1",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_Courses_CourseId1",
                table: "Assignments");

            migrationBuilder.DropIndex(
                name: "IX_Assignments_CourseId1",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "Syllabus",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "CourseId1",
                table: "Assignments");
        }
    }
}
