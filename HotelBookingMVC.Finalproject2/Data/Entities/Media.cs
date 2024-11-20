using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingMVC.Finalproject2.Data.Entities
{
    public class Media
    {
        public Media()
        {
            RoomMediaDetails = new Collection<RoomMediaDetail>();
            HotelMediaDetails = new Collection<HotelMediaDetail>();

        }

        public Guid MediaID { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public string FilePath { get; set; }

        [Required]
        public MediaType MediaType { get; set; }

        // Relationship to Hotel
        //[Required]
        //public Guid HotelID { get; set; }

        //[ForeignKey("HotelID")]
        //public virtual Hotel Hotel { get; set; }

//        // Relationship to Room (optional)
///*        [Required]
//*/        public Guid? RoomID { get; set; }

///*        [ForeignKey("RoomID")]
//*/        public virtual Room Room { get; set; }

        public virtual ICollection<RoomMediaDetail> RoomMediaDetails { get; set; }
        public virtual ICollection<HotelMediaDetail> HotelMediaDetails { get; set; }
    }

    public enum MediaType
    {
        Image,
        Video
    }
}
