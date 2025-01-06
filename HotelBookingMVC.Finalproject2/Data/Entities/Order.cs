using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.Data.Entities
{
    public class Order
    {
        public Order()
        {
            Id = Guid.NewGuid();
            DateCreated = DateTime.Now;
            Bills = new List<Bill>();
        }

        public Guid Id { get; set; }
        public string UserId { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal Total { get; set; }
        public decimal Tax { get; set; }
        public decimal SubTotal { get; set; }

        public string? Note { get; set; }

        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public Guid BillID { get; set; }

        public virtual ICollection<Bill> Bills { get; set; }
    }
}
