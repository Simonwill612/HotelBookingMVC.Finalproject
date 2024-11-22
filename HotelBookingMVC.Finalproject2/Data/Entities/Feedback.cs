using System;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.Data.Entities
{
    public class Feedback
    {
        public Feedback()
        {
            FeedbackID = Guid.NewGuid();
            DateCreated = DateTime.Now;
        }

        // Unique identifier for the feedback
        public Guid FeedbackID { get; set; }

        // Foreign key for the hotel being reviewed
        [Required(ErrorMessage = "Hotel is required.")]
        public Guid HotelID { get; set; }
        public virtual Hotel Hotel { get; set; }

        // Star rating, must be between 1 and 5
        [Required(ErrorMessage = "Star rating is required.")]
        [Range(1, 5, ErrorMessage = "Star rating must be between 1 and 5.")]
        public int StarRating { get; set; }

        // Date the feedback was created
        public DateTime DateCreated { get; set; }

        // Comment provided by the user
        [Required(ErrorMessage = "Comment is required.")]
        [StringLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string Comment { get; set; }

        // User ID of the person providing the feedback
        public string UserID { get; set; }
    }
}