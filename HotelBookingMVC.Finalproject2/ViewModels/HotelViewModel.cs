using HotelBookingMVC.Finalproject2.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.ViewModels
{
    public class HotelViewModel
    {
        public Guid HotelID { get; set; }

        [Required(ErrorMessage = "The Name field is required.")]
        [StringLength(200, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 1)]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Address field is required.")]
        [StringLength(500, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 1)]
        public string Address { get; set; }

        [Required(ErrorMessage = "The City field is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 1)]
        public string City { get; set; }

        [Required(ErrorMessage = "The State field is required.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 1)]
        public string State { get; set; }

        [Required(ErrorMessage = "The Zip Code field is required.")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 1)]
        public string ZipCode { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // If you need to include Room details in the view model, add a property like this:
        // public ICollection<RoomViewModel> Rooms { get; set; }

        // Constructor to set default values if needed
        public HotelViewModel()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;

            Media = new List<MediaViewModel>();
        }
        //public List<Media> Media { get; set; } = new List<Media>();
        public List<MediaViewModel> Media { get; set; }

        // To handle file uploads
        public List<IFormFile>? ImageFiles { get; set; } = new List<IFormFile>();
        public List<IFormFile>? VideoFiles { get; set; } = new List<IFormFile>();
        public List<IFormFile>? MediaFiles { get; set; } = new List<IFormFile>();
        public List<Guid> ExistingMedia { get; set; } = new List<Guid>();


    }
}
