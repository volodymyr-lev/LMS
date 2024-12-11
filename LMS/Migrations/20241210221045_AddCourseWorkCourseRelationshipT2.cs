using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseWorkCourseRelationshipT2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "CourseWorks");

            migrationBuilder.AlterColumn<int>(
                name: "AssignmentId",
                table: "CourseWorks",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AssignmentId",
                table: "CourseWorks",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "CourseWorks",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
