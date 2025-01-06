using HotelBookingMVC.Finalproject2.Data.Entities;
using System.ComponentModel.DataAnnotations;

public class Promotion
{
    public Promotion()
    {
        PromotionID = Guid.NewGuid();
    }

    public Guid PromotionID { get; set; }

    public Guid? HotelID { get; set; }

    [Required]
    [StringLength(50)]
    public string Code { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal DiscountAmount { get; set; }

    public bool IsActive { get; set; }

    [Required]
    public DateTime ExpirationDate { get; set; }

    [Range(1, int.MaxValue)]
    public int QuantityLimit { get; set; }

    public virtual Hotel Hotel { get; set; } // Navigation property
}
