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

        public string HotelName { get; set; }

        public UserViewModel()
        {
            Roles = new List<string>();
            AllRoles = new List<string>();
        }
    }
}
