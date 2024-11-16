using System;
using HotelBookingMVC.Finalproject2.Data;
using HotelBookingMVC.Finalproject2.Data.Entities;
using HotelBookingMVC.Finalproject2.Models;

public class RoomService : IRoomService
{
    private readonly HotelBookingDbContext _context; // Your DbContext

    public RoomService(HotelBookingDbContext context)
    {
        _context = context;
    }

    // Retrieves a room by its RoomID
    public Room GetRoomById(Guid roomId)
    {
        return _context.Rooms
            .FirstOrDefault(r => r.RoomID == roomId);
    }

    public Room GetRoomByNumber(string roomNumber)
    {
        return _context.Rooms
            .FirstOrDefault(r => r .RoomNumber == roomNumber);
    }
}
