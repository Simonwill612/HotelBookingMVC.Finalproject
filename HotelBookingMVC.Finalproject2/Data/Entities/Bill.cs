using System;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.Data.Entities
{
    public class Bill
    {
        public Bill()
        {
            BillID = Guid.NewGuid();
            DateCreatedAt = DateTime.Now;
            DateUpdatedAt = DateTime.Now;
        }

        public Guid BillID { get; set; }

        public string UserID { get; set; }
        // Uncomment if User entity is defined
        //public HotelUser User { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(200)]
        public string Address2 { get; set; }

        [Required]
        [StringLength(100)]
        public string Country { get; set; }

        [Required]
        [StringLength(100)]
        public string State { get; set; }

        [Required]
        [StringLength(20)]
        public string Zip { get; set; }

        public Guid OrderId { get; set; }
        public virtual Order Order { get; set; }

        public DateTime DateCreatedAt { get; set; }
        public DateTime DateUpdatedAt { get; set; }

        public bool IsShippingSameAsBilling { get; set; }
        public bool SaveInfoForNextTime { get; set; }
    }
}
