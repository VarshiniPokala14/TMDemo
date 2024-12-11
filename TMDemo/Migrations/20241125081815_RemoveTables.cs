using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrekMasters.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cancellations");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Reschedules");

            migrationBuilder.AddColumn<DateTime>(
                name: "CancellationDate",
                table: "Bookings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<decimal>(
                name: "ExtraAmount",
                table: "Bookings",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "Bookings",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PaymentSuccess",
                table: "Bookings",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RefundAmount",
                table: "Bookings",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RescheduleReason",
                table: "Bookings",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CancellationDate",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "ExtraAmount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PaymentSuccess",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RefundAmount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "RescheduleReason",
                table: "Bookings");

            migrationBuilder.CreateTable(
                name: "Cancellations",
                columns: table => new
                {
                    CancellationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    BookingId1 = table.Column<int>(type: "int", nullable: true),
                    CancellationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefundAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
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
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reschedules",
                columns: table => new
                {
                    RescheduleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookingId = table.Column<int>(type: "int", nullable: false),
                    BookingId1 = table.Column<int>(type: "int", nullable: true),
                    ExtraAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    NewAvailableDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldAvailableDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RescheduleDate = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "IX_Payments_BookingId",
                table: "Payments",
                column: "BookingId",
                unique: true);

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
    }
}
