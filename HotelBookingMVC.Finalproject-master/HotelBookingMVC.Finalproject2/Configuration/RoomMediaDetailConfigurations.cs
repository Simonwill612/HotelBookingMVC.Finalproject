using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelBookingMVC.Finalproject2.Data.Entities;

namespace HotelBookingMVC.Finalproject2.Configuration
{
    public class RoomMediaDetailConfiguration : IEntityTypeConfiguration<RoomMediaDetail>
    {
        public void Configure(EntityTypeBuilder<RoomMediaDetail> builder)
        {
            builder.HasKey(rmd => new { rmd.RoomId, rmd.MediaId });

            builder.HasOne(rmd => rmd.Room)
                   .WithMany(r => r.RoomMediaDetails)
                   .HasForeignKey(rmd => rmd.RoomId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rmd => rmd.Media)
                   .WithMany(m => m.RoomMediaDetails)
                   .HasForeignKey(rmd => rmd.MediaId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(rmd => rmd.Position)
                   .IsRequired();
        }
    }
}
