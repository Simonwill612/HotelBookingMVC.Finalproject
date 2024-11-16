using HotelBookingMVC.Finalproject2.Data.Entities;

namespace HotelBookingMVC.Finalproject2.ViewModels
{
    public class MediaViewModel
    {
        public MediaViewModel()
        {
            
        }
        public MediaViewModel(Media m)
        {
            MediaID = m.MediaID;
            FileName = m.FileName;
            FilePath = m.FilePath;
            MediaType = m.MediaType;
        }
        public Guid MediaID { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public MediaType MediaType { get; set; }
        public Guid HotelID { get; set; }
        public string HotelName { get; set; }
        public Guid? RoomID { get; set; }  // Correct property name
        public string RoomNumber { get; set; }
    }
}
