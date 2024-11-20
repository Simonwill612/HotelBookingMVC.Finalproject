using System.Collections.ObjectModel;

namespace HotelBookingMVC.Finalproject2.Models
{
    public class ShoppingCart
    {
        public Guid Id { get; set; }

        public string UserID { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Shipping { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
