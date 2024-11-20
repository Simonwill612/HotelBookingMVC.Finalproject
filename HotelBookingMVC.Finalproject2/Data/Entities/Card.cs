using System;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.Data.Entities
{
    public class Card
    {
        public Card()
        {
            CardID = Guid.NewGuid();
        }

        public Guid CardID { get; set; }

        public string UserID { get; set; }
        
        [Required]
        [StringLength(100)]
        public string NameOnCard { get; set; }

        [Required]
        [CreditCard]
        public string CardNumber { get; set; }

        [Required]
        [StringLength(5)]
        public string Expiration { get; set; } // MM/YY format

        [Required]
        [StringLength(3)]
        public string CVV { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
