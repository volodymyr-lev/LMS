using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class AddRulesToThesisAndCourseWorkJunctionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseWorkRule_CourseWorks_CourseWorkId",
                table: "CourseWorkRule");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseWorkRule_Rules_RulesId",
                table: "CourseWorkRule");

            migrationBuilder.DropForeignKey(
                name: "FK_RuleThesis_Rules_RulesId",
                table: "RuleThesis");

            migrationBuilder.DropForeignKey(
                name: "FK_RuleThesis_Theses_ThesisId",
                table: "RuleThesis");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RuleThesis",
                table: "RuleThesis");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseWorkRule",
                table: "CourseWorkRule");

            migrationBuilder.RenameTable(
                name: "RuleThesis",
                newName: "ThesisRules");

            migrationBuilder.RenameTable(
                name: "CourseWorkRule",
                newName: "CourseWorkRules");

            migrationBuilder.RenameIndex(
                name: "IX_RuleThesis_ThesisId",
                table: "ThesisRules",
                newName: "IX_ThesisRules_ThesisId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseWorkRule_RulesId",
                table: "CourseWorkRules",
                newName: "IX_CourseWorkRules_RulesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ThesisRules",
                table: "ThesisRules",
                columns: new[] { "RulesId", "ThesisId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseWorkRules",
                table: "CourseWorkRules",
                columns: new[] { "CourseWorkId", "RulesId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CourseWorkRules_CourseWorks_CourseWorkId",
                table: "CourseWorkRules",
                column: "CourseWorkId",
                principalTable: "CourseWorks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseWorkRules_Rules_RulesId",
                table: "CourseWorkRules",
                column: "RulesId",
                principalTable: "Rules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ThesisRules_Rules_RulesId",
                table: "ThesisRules",
                column: "RulesId",
                principalTable: "Rules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ThesisRules_Theses_ThesisId",
                table: "ThesisRules",
                column: "ThesisId",
                principalTable: "Theses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseWorkRules_CourseWorks_CourseWorkId",
                table: "CourseWorkRules");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseWorkRules_Rules_RulesId",
                table: "CourseWorkRules");

            migrationBuilder.DropForeignKey(
                name: "FK_ThesisRules_Rules_RulesId",
                table: "ThesisRules");

            migrationBuilder.DropForeignKey(
                name: "FK_ThesisRules_Theses_ThesisId",
                table: "ThesisRules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ThesisRules",
                table: "ThesisRules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseWorkRules",
                table: "CourseWorkRules");

            migrationBuilder.RenameTable(
                name: "ThesisRules",
                newName: "RuleThesis");

            migrationBuilder.RenameTable(
                name: "CourseWorkRules",
                newName: "CourseWorkRule");

            migrationBuilder.RenameIndex(
                name: "IX_ThesisRules_ThesisId",
                table: "RuleThesis",
                newName: "IX_RuleThesis_ThesisId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseWorkRules_RulesId",
                table: "CourseWorkRule",
                newName: "IX_CourseWorkRule_RulesId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RuleThesis",
                table: "RuleThesis",
                columns: new[] { "RulesId", "ThesisId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseWorkRule",
                table: "CourseWorkRule",
                columns: new[] { "CourseWorkId", "RulesId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CourseWorkRule_CourseWorks_CourseWorkId",
                table: "CourseWorkRule",
                column: "CourseWorkId",
                principalTable: "CourseWorks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseWorkRule_Rules_RulesId",
                table: "CourseWorkRule",
                column: "RulesId",
                principalTable: "Rules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RuleThesis_Rules_RulesId",
                table: "RuleThesis",
                column: "RulesId",
                principalTable: "Rules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_RuleThesis_Theses_ThesisId",
                table: "RuleThesis",
                column: "ThesisId",
                principalTable: "Theses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
