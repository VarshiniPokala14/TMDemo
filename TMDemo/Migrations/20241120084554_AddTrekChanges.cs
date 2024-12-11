using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrekMasters.Migrations
{
    /// <inheritdoc />
    public partial class AddTrekChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SuitableSeason",
                table: "Availabilities");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Treks",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(40)",
                oldMaxLength: 40);

            migrationBuilder.AddColumn<string>(
                name: "Season",
                table: "Treks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<byte[]>(
                name: "TrekImg",
                table: "Treks",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Season",
                table: "Treks");

            migrationBuilder.DropColumn(
                name: "TrekImg",
                table: "Treks");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Treks",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "SuitableSeason",
                table: "Availabilities",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
