using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.Data.Entities
{
    public class Promotion
    {
        public Promotion()
        {
            PromotionID = Guid.NewGuid();
            //UsedCount = 0;
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

        [Required] // Assuming this is required
        public DateTime ExpirationDate { get; set; } 

        [Range(1, int.MaxValue)] 
        public int QuantityLimit { get; set; } 

        //public int UsedCount { get; set; }


        public virtual Hotel Hotel { get; set; } // Navigation property
    }
}
