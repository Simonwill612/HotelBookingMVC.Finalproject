using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.Data.Entities
{
    public class Room
    {
        public Room()
        {
            RoomID = Guid.NewGuid();
            Bookings = new List<Booking>();
            RoomMediaDetails = new Collection<RoomMediaDetail>();
        }

        public Guid RoomID { get; set; }

        public Guid HotelID { get; set; }
        public virtual Hotel Hotel { get; set; }

        [Required]
        [StringLength(20)]
        public string RoomNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; } // e.g., single, double

        [Required]
        [Range(0, 10000)]
        public decimal PricePerNight { get; set; }

        public string Description { get; set; }

        public bool Availability { get; set; }

        public DateTime DateCreatedAt { get; set; } = DateTime.Now;
        public DateTime DateUpdatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<Booking> Bookings { get; set; }
        public virtual ICollection<RoomMediaDetail> RoomMediaDetails { get; set; }
    }
}
