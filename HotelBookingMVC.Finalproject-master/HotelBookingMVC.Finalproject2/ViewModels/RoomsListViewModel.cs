using System;
using System.Collections.Generic;

namespace HotelBookingMVC.Finalproject2.ViewModels
{
    public class RoomListViewModel
    {
        public Guid HotelID { get; set; }
        public string HotelName { get; set; }
        public List<RoomViewModel> Rooms { get; set; }
    }
}
