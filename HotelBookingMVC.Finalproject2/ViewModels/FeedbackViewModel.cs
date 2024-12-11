using System;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.ViewModels
{
    public class FeedbackViewModel
    {
        public Guid FeedbackID { get; set; }
        public Guid HotelID { get; set; }
        public string HotelName { get; set; }
        public int StarRating { get; set; }
        public string Comment { get; set; }
        public string UserID { get; set; }
        public string UserEmail { get; set; } // Thêm email
        public DateTime DateCreated { get; set; }
    }

}
