using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBookingMVC.Finalproject2.Migrations
{
    /// <inheritdoc />
    public partial class AddPromotions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Promotions_Hotels_HotelID1",
                table: "Promotions");

            migrationBuilder.DropIndex(
                name: "IX_Promotions_HotelID1",
                table: "Promotions");

            migrationBuilder.DropColumn(
                name: "HotelID1",
                table: "Promotions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HotelID1",
                table: "Promotions",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Promotions_HotelID1",
                table: "Promotions",
                column: "HotelID1");

            migrationBuilder.AddForeignKey(
                name: "FK_Promotions_Hotels_HotelID1",
                table: "Promotions",
                column: "HotelID1",
                principalTable: "Hotels",
                principalColumn: "HotelID");
        }
    }
}
