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

        [DataType(DataType.Date)]
        public DateTime CheckOutDate { get; set; }
    }
}
