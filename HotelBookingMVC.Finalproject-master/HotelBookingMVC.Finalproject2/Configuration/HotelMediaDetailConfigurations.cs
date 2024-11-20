using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelBookingMVC.Finalproject2.Data.Entities;

namespace HotelBookingMVC.Finalproject2.Configuration
{
    public class HotelMediaDetailConfiguration : IEntityTypeConfiguration<HotelMediaDetail>
    {
        public void Configure(EntityTypeBuilder<HotelMediaDetail> builder)
        {
            builder.HasKey(rmd => new { rmd.HotelId, rmd.MediaId });

        }
    }
}
