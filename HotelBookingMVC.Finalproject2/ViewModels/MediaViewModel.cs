using HotelBookingMVC.Finalproject2.Data.Entities;

namespace HotelBookingMVC.Finalproject2.ViewModels
{
    public class MediaViewModel
    {
        public MediaViewModel()
        {
        }

        // Constructor that takes a Media object
        public MediaViewModel(Media m)
        {
            if (m == null)
            {
                throw new ArgumentNullException(nameof(m), "Media cannot be null");
            }

            MediaID = m.MediaID;
            FileName = m.FileName ?? "Unknown"; // Default value if null
            FilePath = m.FilePath ?? "Unknown"; // Default value if null
            MediaType = m.MediaType;

            // Optional properties
            var hotelDetail = m.HotelMediaDetails?.FirstOrDefault();
            if (hotelDetail != null)
            {
                HotelID = hotelDetail.HotelId;
                HotelName = hotelDetail.Hotel?.Name ?? "Unknown"; // Avoid null reference
            }
        }

        public Guid MediaID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public MediaType MediaType { get; set; }

        // Properties for related hotel and room information
        public Guid? HotelID { get; set; } // Nullable if not always set
        public string HotelName { get; set; }
        public Guid? RoomID { get; set; }  // Nullable if not always set
        public string RoomNumber { get; set; }
    }
}