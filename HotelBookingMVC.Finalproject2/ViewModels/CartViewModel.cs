using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.ViewModels
{
    public class CartViewModel
    {
        [Required]
        public Guid CartID { get; set; }

        [Required]
        public Guid UserID { get; set; }

        public Guid HotelID { get; set; } // Add this property to hold the hotel ID

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Subtotal must be a positive number.")]
        public decimal Subtotal { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discount must be a positive number.")]
        public decimal Discount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Shipping must be a positive number.")]
        public decimal Shipping { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tax must be a positive number.")]
        public decimal Tax { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Total must be a positive number.")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "At least one item must be in the cart.")]
        public virtual ICollection<CartItemViewModel> CartItems { get; set; } = new List<CartItemViewModel>();

        [Required(ErrorMessage = "Check-In Date is required.")]
        public DateTime? CheckInDate { get; set; }

        [Required(ErrorMessage = "Check-Out Date is required.")]
        public DateTime? CheckOutDate { get; set; }

        [Required(ErrorMessage = "The Room field is required.")]
        public string RoomID { get; set; }

        [Required(ErrorMessage = "The Booking Status field is required.")]
        public string BookingStatus { get; set; }

        [Required(ErrorMessage = "At least one payment must be provided.")]
        public ICollection<PaymentViewModel> Payments { get; set; } = new List<PaymentViewModel>();

    }
}
