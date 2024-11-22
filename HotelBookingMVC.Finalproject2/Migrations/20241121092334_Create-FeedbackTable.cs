using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelBookingMVC.Finalproject2.Migrations
{
    /// <inheritdoc />
    public partial class CreateFeedbackTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_Hotels_HotelID",
                table: "Feedback");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Feedback",
                table: "Feedback");

            migrationBuilder.RenameTable(
                name: "Feedback",
                newName: "Feedbacks");

            migrationBuilder.RenameIndex(
                name: "IX_Feedback_HotelID",
                table: "Feedbacks",
                newName: "IX_Feedbacks_HotelID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Feedbacks",
                table: "Feedbacks",
                column: "FeedbackID");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Hotels_HotelID",
                table: "Feedbacks",
                column: "HotelID",
                principalTable: "Hotels",
                principalColumn: "HotelID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Hotels_HotelID",
                table: "Feedbacks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Feedbacks",
                table: "Feedbacks");

            migrationBuilder.RenameTable(
                name: "Feedbacks",
                newName: "Feedback");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_HotelID",
                table: "Feedback",
                newName: "IX_Feedback_HotelID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Feedback",
                table: "Feedback",
                column: "FeedbackID");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_Hotels_HotelID",
                table: "Feedback",
                column: "HotelID",
                principalTable: "Hotels",
                principalColumn: "HotelID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
