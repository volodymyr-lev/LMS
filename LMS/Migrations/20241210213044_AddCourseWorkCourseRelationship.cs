using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseWorkCourseRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CourseId",
                table: "Theses",
                type: "integer",
                nullable: true);

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

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "CourseWorks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                name: "CourseId",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "CourseWorks");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "CourseWorks");

            migrationBuilder.DropColumn(
                name: "CourseWorkId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "ThesisId",
                table: "Courses");
        }
    }
}
