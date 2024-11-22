using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TMDemo.Migrations
{
    /// <inheritdoc />
    public partial class BookingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    BookingId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TrekId = table.Column<int>(type: "int", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberOfPeople = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    TrekStartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrekId1 = table.Column<int>(type: "int", nullable: true),
                    UserDetailId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.BookingId);
                    table.ForeignKey(
                        name: "FK_Bookings_Treks_TrekId",
                        column: x => x.TrekId,
                        principalTable: "Treks",
                        principalColumn: "TrekId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_Treks_TrekId1",
                        column: x => x.TrekId1,
                        principalTable: "Treks",
                        principalColumn: "TrekId");
                    table.ForeignKey(
                        name: "FK_Bookings_Users_UserDetailId",
                        column: x => x.UserDetailId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bookings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TrekId",
                table: "Bookings",
                column: "TrekId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_TrekId1",
                table: "Bookings",
                column: "TrekId1");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserDetailId",
                table: "Bookings",
                column: "UserDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_UserId",
                table: "Bookings",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");
        }
    }
}
