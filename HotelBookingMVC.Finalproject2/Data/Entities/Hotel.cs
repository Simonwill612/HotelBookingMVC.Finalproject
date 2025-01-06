using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingMVC.Finalproject2.Data.Entities
{
    public class Hotel
    {
        public Hotel()
        {
            HotelID = Guid.NewGuid();
            //Rooms = new List<Room>();
            //Media = new List<Media>();

            HotelMediaDetails = new Collection<HotelMediaDetail>();
        }

        public Guid HotelID { get; set; }

        public string? UserID { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(500)]
        public string Address { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; }

        [Required]
        [StringLength(100)]
        public string State { get; set; }

        [Required]
        [StringLength(20)]
        public string ZipCode { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        //public ICollection<Room> Rooms { get; set; }
        //public ICollection<Media> Media { get; set; } = new List<Media>();

        public virtual ICollection<HotelMediaDetail> HotelMediaDetails { get; set; }
        public virtual ICollection<Promotion> Promotions { get; set; }
        public virtual ICollection<Feedback> Feedbacks { get; set; }
        public virtual ICollection<Booking> Bookings { get; set; }

        [NotMapped]
        public object CreatedByUser { get; set; }
    }
}
