using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelBookingMVC.Finalproject2.Data.Entities;

namespace HotelBookingMVC.Finalproject2.Configuration
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasKey(r => r.RoomID);

            builder.Property(r => r.RoomNumber)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(r => r.Type)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(r => r.PricePerNight)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(r => r.Description)
                   .HasMaxLength(1000);

            builder.Property(r => r.DateCreatedAt)
                   .IsRequired();

            builder.Property(r => r.DateUpdatedAt)
                   .IsRequired();

            builder.ToTable("Rooms");
        }
    }
}
