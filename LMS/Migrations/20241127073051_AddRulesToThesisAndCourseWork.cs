using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class AddRulesToThesisAndCourseWork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MentorId",
                table: "Theses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "Theses",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LectorId",
                table: "CourseWorks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Score",
                table: "CourseWorks",
                type: "double precision",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CourseWorkRule",
                columns: table => new
                {
                    CourseWorkId = table.Column<int>(type: "integer", nullable: false),
                    RulesId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseWorkRule", x => new { x.CourseWorkId, x.RulesId });
                    table.ForeignKey(
                        name: "FK_CourseWorkRule_CourseWorks_CourseWorkId",
                        column: x => x.CourseWorkId,
                        principalTable: "CourseWorks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseWorkRule_Rules_RulesId",
                        column: x => x.RulesId,
                        principalTable: "Rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RuleThesis",
                columns: table => new
                {
                    RulesId = table.Column<int>(type: "integer", nullable: false),
                    ThesisId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleThesis", x => new { x.RulesId, x.ThesisId });
                    table.ForeignKey(
                        name: "FK_RuleThesis_Rules_RulesId",
                        column: x => x.RulesId,
                        principalTable: "Rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RuleThesis_Theses_ThesisId",
                        column: x => x.ThesisId,
                        principalTable: "Theses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Theses_MentorId",
                table: "Theses",
                column: "MentorId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseWorks_LectorId",
                table: "CourseWorks",
                column: "LectorId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseWorkRule_RulesId",
                table: "CourseWorkRule",
                column: "RulesId");

            migrationBuilder.CreateIndex(
                name: "IX_RuleThesis_ThesisId",
                table: "RuleThesis",
                column: "ThesisId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseWorks_AspNetUsers_LectorId",
                table: "CourseWorks",
                column: "LectorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Theses_AspNetUsers_MentorId",
                table: "Theses",
                column: "MentorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseWorks_AspNetUsers_LectorId",
                table: "CourseWorks");

            migrationBuilder.DropForeignKey(
                name: "FK_Theses_AspNetUsers_MentorId",
                table: "Theses");

            migrationBuilder.DropTable(
                name: "CourseWorkRule");

            migrationBuilder.DropTable(
                name: "RuleThesis");

            migrationBuilder.DropIndex(
                name: "IX_Theses_MentorId",
                table: "Theses");

            migrationBuilder.DropIndex(
                name: "IX_CourseWorks_LectorId",
                table: "CourseWorks");

            migrationBuilder.DropColumn(
                name: "MentorId",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "LectorId",
                table: "CourseWorks");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "CourseWorks");
        }
    }
}
