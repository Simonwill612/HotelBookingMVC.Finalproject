using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using HotelBookingMVC.Finalproject2.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.ViewModels
{
    public class RoomViewModel
    {
        public RoomViewModel()
        {
            DateCreatedAt = DateTime.Now;
            DateUpdatedAt = DateTime.Now;
            MediaViewModels = new List<MediaViewModel>();
            RoomMediaDetails = new List<MediaViewModel>();
        }

        public Guid RoomID { get; set; }
        public Guid HotelID { get; set; }

        [Required]
        [StringLength(20)]
        public string RoomNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; } // e.g., single, double

        public string Description { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal PricePerNight { get; set; }

        public bool Availability { get; set; }
        public DateTime DateCreatedAt { get; set; }
        public DateTime DateUpdatedAt { get; set; }

        // To handle file uploads
        public List<IFormFile>? ImageFiles { get; set; } = new List<IFormFile>();
        public List<IFormFile>? VideoFiles { get; set; } = new List<IFormFile>();
        public List<IFormFile>? MediaFiles { get; set; } = new List<IFormFile>();
        public List<Guid>? ExistingMedia { get; set; } = new List<Guid>();

        // Add MediaViewModels property
        public List<MediaViewModel> MediaViewModels { get; set; }
        public List<MediaViewModel> RoomMediaDetails { get; internal set; }
    }
}
