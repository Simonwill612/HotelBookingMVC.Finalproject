using System;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.Data.Entities
{
    public class Payment
    {
        public Payment()
        {
            PaymentID = Guid.NewGuid();
        }

        public Guid PaymentID { get; set; }

        public Guid BookingID { get; set; }
        public Booking Booking { get; set; }

        [Required]
        [Range(0, 100000)]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } // e.g., credit card, PayPal

        [Required]
        [StringLength(50)]
        public string PaymentStatus { get; set; } // e.g., paid, pending

        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
