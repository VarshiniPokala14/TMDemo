using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMDemo.Migrations
{
    /// <inheritdoc />
    public partial class ModifyTrekPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrekPlans_TrekId",
                table: "TrekPlans");

            migrationBuilder.DropColumn(
                name: "Activities",
                table: "TrekPlans");

            migrationBuilder.AddColumn<string>(
                name: "ActivityDescription",
                table: "TrekPlans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "TrekPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TrekPlans_TrekId",
                table: "TrekPlans",
                column: "TrekId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TrekPlans_TrekId",
                table: "TrekPlans");

            migrationBuilder.DropColumn(
                name: "ActivityDescription",
                table: "TrekPlans");

            migrationBuilder.DropColumn(
                name: "Day",
                table: "TrekPlans");

            migrationBuilder.AddColumn<string>(
                name: "Activities",
                table: "TrekPlans",
                type: "nvarchar(max)",
                maxLength: 5000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TrekPlans_TrekId",
                table: "TrekPlans",
                column: "TrekId",
                unique: true);
        }
    }
}
