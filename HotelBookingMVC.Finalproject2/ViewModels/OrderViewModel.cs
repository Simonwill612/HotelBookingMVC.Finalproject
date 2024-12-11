using System;
using System.Collections.Generic;

namespace HotelBookingMVC.Finalproject2.ViewModels
{
    public class OrderViewModel
    {
        public Guid Id { get; set; }
        public DateTime DateCreated { get; set; }
        public decimal Total { get; set; }
        public decimal Tax { get; set; }
        public decimal SubTotal { get; set; }
        public string? Note { get; set; }
        public string Address { get; set; }

        // Added properties
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address2 { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public bool IsShippingSameAsBilling { get; set; }
        public bool SaveInfoForNextTime { get; set; }

        // New property for order confirmation
        public string City { get; set; } // Include this for address details
        public List<CartItemViewModel> CartItems { get; internal set; }
    }
}
