using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class MovedStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "CourseWorks");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "CourseWorks");

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "Assignments",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Assignments",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Score",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Assignments");

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "Theses",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Theses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "CourseWorks",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "CourseWorks",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
