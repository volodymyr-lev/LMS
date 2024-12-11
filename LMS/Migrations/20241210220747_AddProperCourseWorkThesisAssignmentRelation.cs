using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class AddProperCourseWorkThesisAssignmentRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseWorks_Courses_CourseId",
                table: "CourseWorks");

            migrationBuilder.DropForeignKey(
                name: "FK_Theses_Courses_CourseId",
                table: "Theses");

            migrationBuilder.DropIndex(
                name: "IX_Theses_CourseId",
                table: "Theses");

            migrationBuilder.DropIndex(
                name: "IX_CourseWorks_CourseId",
                table: "CourseWorks");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "CourseWorks");

            migrationBuilder.DropColumn(
                name: "CourseWorkId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "ThesisId",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Theses",
                newName: "AssignmentId");

            migrationBuilder.AddColumn<int>(
                name: "AssignmentId",
                table: "CourseWorks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Theses_AssignmentId",
                table: "Theses",
                column: "AssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseWorks_AssignmentId",
                table: "CourseWorks",
                column: "AssignmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseWorks_Assignments_AssignmentId",
                table: "CourseWorks",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Theses_Assignments_AssignmentId",
                table: "Theses",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseWorks_Assignments_AssignmentId",
                table: "CourseWorks");

            migrationBuilder.DropForeignKey(
                name: "FK_Theses_Assignments_AssignmentId",
                table: "Theses");

            migrationBuilder.DropIndex(
                name: "IX_Theses_AssignmentId",
                table: "Theses");

            migrationBuilder.DropIndex(
                name: "IX_CourseWorks_AssignmentId",
                table: "CourseWorks");

            migrationBuilder.DropColumn(
                name: "AssignmentId",
                table: "CourseWorks");

            migrationBuilder.RenameColumn(
                name: "AssignmentId",
                table: "Theses",
                newName: "CourseId");

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Theses",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "CourseWorks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CourseWorkId",
                table: "Courses",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ThesisId",
                table: "Courses",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Theses_CourseId",
                table: "Theses",
                column: "CourseId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseWorks_CourseId",
                table: "CourseWorks",
                column: "CourseId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseWorks_Courses_CourseId",
                table: "CourseWorks",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Theses_Courses_CourseId",
                table: "Theses",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
