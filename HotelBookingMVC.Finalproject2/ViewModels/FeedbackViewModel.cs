using System;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.ViewModels
{
    public class FeedbackViewModel
    {
        [Required(ErrorMessage = "Hotel is required.")]
        public Guid HotelID { get; set; }

        [Required(ErrorMessage = "Star rating is required.")]
        [Range(1, 5, ErrorMessage = "Star rating must be between 1 and 5.")]
        public int StarRating { get; set; }

        [Required(ErrorMessage = "Comment is required.")]
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string Comment { get; set; }
    }
}
