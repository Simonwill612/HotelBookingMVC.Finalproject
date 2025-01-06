using HotelBookingMVC.Finalproject2.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.ViewModels
{
    public class CartItemViewModel
    {
        public Guid CartItemID { get; set; }
        public Guid RoomID { get; set; } // Renamed from ProductId
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string RoomNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime CheckInDate { get; set; }
        public decimal Discount { get; set; }
        public decimal Total => (Price * Quantity) - Discount;  // Calculate total after discount

        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }
        public string FilePath { get; set; } // Add this line
        public Guid HotelID { get; set; }
    }
}
