using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.ViewModels
{
    public class BookingViewModel
    {
        [Key]
        public Guid BookingID { get; set; }

        public Guid CartID { get; set; }

        [Required]
        public string UserID { get; set; }
        [Required]
        public string RoomNumber { get; set; }

        [Required]
        public Guid RoomID { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-In Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CheckInDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Check-Out Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Total Price must be a positive value.")]
        [Display(Name = "Total Price")]
        [DataType(DataType.Currency)]
        public decimal TotalPrice { get; set; }

        [Required]
        [Display(Name = "Booking Status")]
        public string Status { get; set; } // Could be "Confirmed", "Pending", "Cancelled"

        public ICollection<PaymentViewModel> Payments { get; set; }

        // Related Room details (optional)
        public RoomViewModel Room { get; set; }

        // Optional properties for additional financial information
        [Range(0, double.MaxValue, ErrorMessage = "Subtotal must be a positive value.")]
        [Display(Name = "Subtotal")]
        [DataType(DataType.Currency)]
        public decimal Subtotal { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Discount must be a positive value.")]
        [Display(Name = "Discount")]
        [DataType(DataType.Currency)]
        public decimal Discount { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Shipping must be a positive value.")]
        [Display(Name = "Shipping")]
        [DataType(DataType.Currency)]
        public decimal Shipping { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Tax must be a positive value.")]
        [Display(Name = "Tax")]
        [DataType(DataType.Currency)]
        public decimal Tax { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Total must be a positive value.")]
        [Display(Name = "Final Total")]
        [DataType(DataType.Currency)]
        public decimal Total
        {
            get
            {
                return (Subtotal - Discount) + Shipping + Tax;
            }
        }
    }
}
