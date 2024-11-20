using System;
using System.Collections.Generic;
using System.Linq;
using HotelBookingMVC.Finalproject2.Data;
using HotelBookingMVC.Finalproject2.Data.Entities;
using HotelBookingMVC.Finalproject2.Models;

public class BookingService : IBookingService
{
    private readonly HotelBookingDbContext _context; // Your DbContext

    public BookingService(HotelBookingDbContext context)
    {
        _context = context;
    }

    // Gets the list of booked dates for a room
    public List<string> GetBookedDatesForRoom(Guid roomId)
    {
        var bookings = _context.Bookings
            .Where(b => b.RoomID == roomId && b.Status == "Confirmed")
            .ToList();

        List<string> bookedDates = new List<string>();
        foreach (var booking in bookings)
        {
            for (var date = booking.CheckInDate; date < booking.CheckOutDate; date = date.AddDays(1))
            {
                bookedDates.Add(date.ToString("yyyy-MM-dd"));
            }
        }
        return bookedDates;
    }

    // Checks if the room is available for a given date range
    public bool IsRoomAvailable(Guid roomId, DateTime checkIn, DateTime checkOut)
    {
        return !_context.Bookings
            .Any(b => b.RoomID == roomId && b.Status == "Confirmed" &&
                      ((b.CheckInDate <= checkIn && b.CheckOutDate > checkIn) ||
                       (b.CheckInDate < checkOut && b.CheckOutDate >= checkOut)));
    }

    // Creates a new booking and returns the generated BookingID
    public Guid CreateBooking(Booking booking)
    {
        booking.BookingID = Guid.NewGuid();
        _context.Bookings.Add(booking);
        _context.SaveChanges();
        return booking.BookingID;
    }
}
