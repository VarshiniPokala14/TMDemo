using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrekMasters.Migrations
{
    /// <inheritdoc />
    public partial class AddRescheduleCancellationsTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cancellations",
                columns: table => new
                {
                    CancellationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    CancellationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    BookingId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cancellations", x => x.CancellationId);
                    table.ForeignKey(
                        name: "FK_Cancellations_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Cancellations_Bookings_BookingId1",
                        column: x => x.BookingId1,
                        principalTable: "Bookings",
                        principalColumn: "BookingId");
                });

            migrationBuilder.CreateTable(
                name: "Reschedules",
                columns: table => new
                {
                    RescheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    OldAvailableDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NewAvailableDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExtraAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RescheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BookingId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reschedules", x => x.RescheduleId);
                    table.ForeignKey(
                        name: "FK_Reschedules_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reschedules_Bookings_BookingId1",
                        column: x => x.BookingId1,
                        principalTable: "Bookings",
                        principalColumn: "BookingId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cancellations_BookingId",
                table: "Cancellations",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Cancellations_BookingId1",
                table: "Cancellations",
                column: "BookingId1",
                unique: true,
                filter: "[BookingId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Reschedules_BookingId",
                table: "Reschedules",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_Reschedules_BookingId1",
                table: "Reschedules",
                column: "BookingId1",
                unique: true,
                filter: "[BookingId1] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cancellations");

            migrationBuilder.DropTable(
                name: "Reschedules");
        }
    }
}
