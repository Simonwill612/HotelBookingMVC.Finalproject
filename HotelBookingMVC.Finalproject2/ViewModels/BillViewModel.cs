
namespace HotelBookingMVC.Finalproject2.ViewModels
{
    public class BillViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address2 { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public bool IsShippingSameAsBilling { get; set; }
        public bool SaveInfoForNextTime { get; set; }
        public List<CartItemViewModel> CartItems { get; set; }
    }
}
