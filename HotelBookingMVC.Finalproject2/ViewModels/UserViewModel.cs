namespace HotelBookingMVC.Finalproject2.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public IList<string> Roles { get; set; }
        public IList<string> AllRoles { get; set; }

        // Các trường bổ sung
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string ProfilePictureFileName { get; set; }

        public UserViewModel()
        {
            Roles = new List<string>();
            AllRoles = new List<string>();
        }
    }
}
