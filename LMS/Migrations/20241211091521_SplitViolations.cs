using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class SplitViolations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourseWorkVerification",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourseWorkId = table.Column<int>(type: "integer", nullable: false),
                    VerificationDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Result = table.Column<string>(type: "text", nullable: false),
                    Comments = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseWorkVerification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseWorkVerification_CourseWorks_CourseWorkId",
                        column: x => x.CourseWorkId,
                        principalTable: "CourseWorks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ViolationCourses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    CourseWorkVerificationId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViolationCourses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ViolationCourses_CourseWorkVerification_CourseWorkVerificat~",
                        column: x => x.CourseWorkVerificationId,
                        principalTable: "CourseWorkVerification",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseWorkVerification_CourseWorkId",
                table: "CourseWorkVerification",
                column: "CourseWorkId");

            migrationBuilder.CreateIndex(
                name: "IX_ViolationCourses_CourseWorkVerificationId",
                table: "ViolationCourses",
                column: "CourseWorkVerificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ViolationCourses");

            migrationBuilder.DropTable(
                name: "CourseWorkVerification");
        }
    }
}
