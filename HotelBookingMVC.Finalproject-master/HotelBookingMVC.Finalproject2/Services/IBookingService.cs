using System;
using System.Collections.Generic;
using HotelBookingMVC.Finalproject2.Data.Entities;
using HotelBookingMVC.Finalproject2.Models;

public interface IBookingService
{
    List<string> GetBookedDatesForRoom(Guid roomId);
    bool IsRoomAvailable(Guid roomId, DateTime checkIn, DateTime checkOut);
    Guid CreateBooking(Booking booking);
}
