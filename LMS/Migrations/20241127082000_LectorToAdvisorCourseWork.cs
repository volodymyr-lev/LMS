using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class LectorToAdvisorCourseWork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseWorks_AspNetUsers_LectorId",
                table: "CourseWorks");

            migrationBuilder.RenameColumn(
                name: "LectorId",
                table: "CourseWorks",
                newName: "AdvisorId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseWorks_LectorId",
                table: "CourseWorks",
                newName: "IX_CourseWorks_AdvisorId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseWorks_AspNetUsers_AdvisorId",
                table: "CourseWorks",
                column: "AdvisorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseWorks_AspNetUsers_AdvisorId",
                table: "CourseWorks");

            migrationBuilder.RenameColumn(
                name: "AdvisorId",
                table: "CourseWorks",
                newName: "LectorId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseWorks_AdvisorId",
                table: "CourseWorks",
                newName: "IX_CourseWorks_LectorId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseWorks_AspNetUsers_LectorId",
                table: "CourseWorks",
                column: "LectorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
