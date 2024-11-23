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

        // New properties for Manager role
        public Guid? HotelID { get; set; } // Nullable in case the user is not a manager
        public string HotelName { get; set; } // The name of the hotel associated with the manager

        public UserViewModel()
        {
            Roles = new List<string>();
            AllRoles = new List<string>();
        }
    }
}
