using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class CWViolations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseWorkVerification_CourseWorks_CourseWorkId",
                table: "CourseWorkVerification");

            migrationBuilder.DropForeignKey(
                name: "FK_ViolationCourses_CourseWorkVerification_CourseWorkVerificat~",
                table: "ViolationCourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseWorkVerification",
                table: "CourseWorkVerification");

            migrationBuilder.RenameTable(
                name: "CourseWorkVerification",
                newName: "CourseWorkVerifications");

            migrationBuilder.RenameIndex(
                name: "IX_CourseWorkVerification_CourseWorkId",
                table: "CourseWorkVerifications",
                newName: "IX_CourseWorkVerifications_CourseWorkId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseWorkVerifications",
                table: "CourseWorkVerifications",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseWorkVerifications_CourseWorks_CourseWorkId",
                table: "CourseWorkVerifications",
                column: "CourseWorkId",
                principalTable: "CourseWorks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ViolationCourses_CourseWorkVerifications_CourseWorkVerifica~",
                table: "ViolationCourses",
                column: "CourseWorkVerificationId",
                principalTable: "CourseWorkVerifications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseWorkVerifications_CourseWorks_CourseWorkId",
                table: "CourseWorkVerifications");

            migrationBuilder.DropForeignKey(
                name: "FK_ViolationCourses_CourseWorkVerifications_CourseWorkVerifica~",
                table: "ViolationCourses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseWorkVerifications",
                table: "CourseWorkVerifications");

            migrationBuilder.RenameTable(
                name: "CourseWorkVerifications",
                newName: "CourseWorkVerification");

            migrationBuilder.RenameIndex(
                name: "IX_CourseWorkVerifications_CourseWorkId",
                table: "CourseWorkVerification",
                newName: "IX_CourseWorkVerification_CourseWorkId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseWorkVerification",
                table: "CourseWorkVerification",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseWorkVerification_CourseWorks_CourseWorkId",
                table: "CourseWorkVerification",
                column: "CourseWorkId",
                principalTable: "CourseWorks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ViolationCourses_CourseWorkVerification_CourseWorkVerificat~",
                table: "ViolationCourses",
                column: "CourseWorkVerificationId",
                principalTable: "CourseWorkVerification",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
