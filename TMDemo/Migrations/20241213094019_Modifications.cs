using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrekMasters.Migrations
{
    /// <inheritdoc />
    public partial class Modifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Treks_TrekId",
                table: "Bookings");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Treks_TrekId",
                table: "Bookings",
                column: "TrekId",
                principalTable: "Treks",
                principalColumn: "TrekId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Treks_TrekId",
                table: "Bookings");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Treks_TrekId",
                table: "Bookings",
                column: "TrekId",
                principalTable: "Treks",
                principalColumn: "TrekId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
