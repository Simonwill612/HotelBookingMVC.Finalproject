using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelBookingMVC.Finalproject2.Data.Entities;

namespace HotelBookingMVC.Finalproject2.Configurations
{
    public class MediaConfiguration : IEntityTypeConfiguration<Media>
    {
        public void Configure(EntityTypeBuilder<Media> builder)
        {
            builder.HasKey(m => m.MediaID);

            builder.Property(m => m.FileName)
                   .IsRequired()
                   .HasMaxLength(255);

            builder.Property(m => m.FilePath)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.Property(m => m.MediaType)
                   .IsRequired();

            //builder.HasOne(m => m.Hotel)
            //       .WithMany(h => h.Media)
            //       .HasForeignKey(m => m.HotelID)
            //       .OnDelete(DeleteBehavior.Cascade);

            //builder.HasMany(m => m.RoomMediaDetails)
            //       .WithOne(rmd => rmd.Media)
            //       .HasForeignKey(rmd => rmd.MediaId)
            //       .OnDelete(DeleteBehavior.Cascade);
        }
    }
}