using System;
using HotelBookingMVC.Finalproject2.Data.Entities;
using HotelBookingMVC.Finalproject2.Models;

public interface IRoomService
{
    Room GetRoomById(Guid roomId);
    Room GetRoomByNumber(string roomNumber);
}
