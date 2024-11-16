using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using HotelBookingMVC.Finalproject2.Models;
namespace HotelBookingMVC.Finalproject2.Data
{
    public class HotelIdentityDBContext : IdentityDbContext<HotelUser>
    {
        public HotelIdentityDBContext(DbContextOptions<HotelIdentityDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
