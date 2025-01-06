using System;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.ViewModels
{
    public class FeedbackViewModel
    {
        public Guid FeedbackID { get; set; }
        public Guid HotelID { get; set; }
        public string HotelName { get; set; }

        [Required(ErrorMessage = "Star rating is required.")]
        [Range(1, 5, ErrorMessage = "Please select a valid star rating between 1 and 5.")] public int StarRating { get; set; }

        [Required(ErrorMessage = "Comment is required.")]
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string Comment { get; set; }
        public string UserID { get; set; }
        public string UserEmail { get; set; } // Thêm email
        public DateTime DateCreated { get; set; }
    }

}
