using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingMVC.Finalproject2.Models
{
    public class HotelUser : IdentityUser
    {
        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }


        [StringLength(200)]
        public string? Address { get; set; }

        [StringLength(200)]
        public string? Address2 { get; set; }

        [StringLength(100)]
        public string? Country { get; set; }

        [StringLength(100)]
        public string? State { get; set; }

        [StringLength(20)]
        public string? Zip { get; set; }


        public string? ImageName { get; set; }

        public string? ProfilePictureLocation { get; set; }

        public string? ProfilePictureFileName { get; set; }
        public virtual ICollection<IdentityUserRole<string>> Roles { get; } = new List<IdentityUserRole<string>>();


    }
}
