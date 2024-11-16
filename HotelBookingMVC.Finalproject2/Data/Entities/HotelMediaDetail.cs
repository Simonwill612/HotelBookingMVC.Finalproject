namespace HotelBookingMVC.Finalproject2.Data.Entities
{
    public class HotelMediaDetail
    {
        public Guid HotelId { get; set; }
        public virtual Hotel Hotel { get; set; }

        public Guid MediaId { get; set; }
        public virtual Media Media { get; set; }

        public int Position { get; set; }
    }
}
