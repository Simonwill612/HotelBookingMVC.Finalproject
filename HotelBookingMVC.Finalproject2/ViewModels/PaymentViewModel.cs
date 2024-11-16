using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.ViewModels
{
    public class PaymentViewModel
    {
        public Guid PaymentID { get; set; } = Guid.NewGuid();

        [Required]
        [Display(Name = "Cardholder Name")]
        public string CardName { get; set; }

        [Required]
        [Display(Name = "Card Number")]
        [CreditCard]  // This annotation validates the format of the credit card number
        [StringLength(16, ErrorMessage = "Card number must be 16 digits.", MinimumLength = 16)]
        public string CardNumber { get; set; }

        [Required]
        [Display(Name = "Expiration Date (MM/YY)")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/?([0-9]{2})$", ErrorMessage = "Expiration date must be in MM/YY format.")]
        public string ExpirationDate { get; set; }

        [Required]
        [Display(Name = "CVV")]
        [StringLength(3, ErrorMessage = "CVV must be 3 digits.", MinimumLength = 3)]
        public string CVV { get; set; }

        [Required]
        [Display(Name = "Amount")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        public Guid BookingID { get; set; } // Reference to Booking if needed
    }
}
