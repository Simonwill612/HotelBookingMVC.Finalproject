using HotelBookingMVC.Finalproject2.Data.Entities;
using System.ComponentModel.DataAnnotations;

public class Booking
{
    public Booking()
    {
        BookingID = Guid.NewGuid();
        Payments = new List<Payment>();
    }

    public Guid BookingID { get; set; }

    public string UserID { get; set; }
    public Guid RoomID { get; set; }
    public Room Room { get; set; }

    public Guid HotelID { get; set; }  // Add HotelID to link to Hotel
    public Hotel Hotel { get; set; }    // Navigation property to Hotel

    public DateTime BookingDate { get; set; } = DateTime.UtcNow;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }

    [Required]
    [StringLength(50)]
    public string Status { get; set; }

    [Required]
    [Range(0, 100000)]
    public decimal TotalPrice { get; set; }

    public virtual ICollection<Payment> Payments { get; set; }
}
