using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrekMasters.Migrations
{
    /// <inheritdoc />
    public partial class TrekReviewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrekReviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TrekId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrekReviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_TrekReviews_Treks_TrekId",
                        column: x => x.TrekId,
                        principalTable: "Treks",
                        principalColumn: "TrekId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrekReviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrekReviews_TrekId",
                table: "TrekReviews",
                column: "TrekId");

            migrationBuilder.CreateIndex(
                name: "IX_TrekReviews_UserId_TrekId",
                table: "TrekReviews",
                columns: new[] { "UserId", "TrekId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TrekReviews");
        }
    }
}
