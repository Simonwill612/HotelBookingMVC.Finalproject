using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.ViewModels
{
    public class PromotionViewModel
    {
        public Guid PromotionID { get; set; }
        //public Guid? HotelID { get; set; }
        public string Code { get; set; }
        public decimal DiscountAmount { get; set; }
        public bool IsActive { get; set; }
        public DateTime ExpirationDate { get; set; } = DateTime.UtcNow;
        public int QuantityLimit { get; set; }
        public List<Guid> HotelIDs { get; set; } = new List<Guid>(); // Duy trì danh sách các khách sạn

    }
}
