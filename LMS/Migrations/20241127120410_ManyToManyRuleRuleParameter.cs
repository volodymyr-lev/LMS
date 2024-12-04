using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMS.Migrations
{
    /// <inheritdoc />
    public partial class ManyToManyRuleRuleParameter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RuleParameters_Rules_RuleId",
                table: "RuleParameters");

            migrationBuilder.DropIndex(
                name: "IX_RuleParameters_RuleId",
                table: "RuleParameters");

            migrationBuilder.DropColumn(
                name: "RuleId",
                table: "RuleParameters");

            migrationBuilder.CreateTable(
                name: "RuleRuleParameters",
                columns: table => new
                {
                    RuleId = table.Column<int>(type: "integer", nullable: false),
                    RuleParameterId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RuleRuleParameters", x => new { x.RuleId, x.RuleParameterId });
                    table.ForeignKey(
                        name: "FK_RuleRuleParameters_RuleParameters_RuleParameterId",
                        column: x => x.RuleParameterId,
                        principalTable: "RuleParameters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RuleRuleParameters_Rules_RuleId",
                        column: x => x.RuleId,
                        principalTable: "Rules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RuleRuleParameters_RuleParameterId",
                table: "RuleRuleParameters",
                column: "RuleParameterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RuleRuleParameters");

            migrationBuilder.AddColumn<int>(
                name: "RuleId",
                table: "RuleParameters",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RuleParameters_RuleId",
                table: "RuleParameters",
                column: "RuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_RuleParameters_Rules_RuleId",
                table: "RuleParameters",
                column: "RuleId",
                principalTable: "Rules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
