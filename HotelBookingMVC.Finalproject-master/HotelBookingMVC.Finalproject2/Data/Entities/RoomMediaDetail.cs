namespace HotelBookingMVC.Finalproject2.Data.Entities
{
    public class RoomMediaDetail
    {
        public Guid RoomId { get; set; }
        public virtual Room Room { get; set; }

        public Guid MediaId { get; set; }
        public virtual Media Media { get; set; }

        public int Position { get; set; }
        //public Guid RoomMediaDetailId { get; internal set; }
    }
}
