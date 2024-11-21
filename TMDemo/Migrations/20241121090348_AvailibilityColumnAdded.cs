using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMDemo.Migrations
{
    /// <inheritdoc />
    public partial class AvailibilityColumnAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Month",
                table: "Availabilities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Month",
                table: "Availabilities");
        }
    }
}
