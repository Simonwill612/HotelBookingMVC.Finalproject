using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBookingMVC.Finalproject2.Migrations
{
    /// <inheritdoc />
    public partial class Edit_OderTabler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BillId",
                table: "Orders",
                newName: "BillID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BillID",
                table: "Orders",
                newName: "BillId");
        }
    }
}
