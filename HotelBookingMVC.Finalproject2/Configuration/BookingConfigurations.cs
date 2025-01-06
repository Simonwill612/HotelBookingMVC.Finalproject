using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelBookingMVC.Finalproject2.Data.Entities;

namespace HotelBookingMVC.Finalproject2.Configuration
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.HasKey(b => b.BookingID);

            builder.HasOne(b => b.Room)
                   .WithMany(r => r.Bookings)
                   .HasForeignKey(b => b.RoomID)
                   .OnDelete(DeleteBehavior.NoAction); // Avoid cascade delete to prevent unintentional data loss.

            // Configure the relationship between Booking and Payment
            builder.HasMany(b => b.Payments) // A booking can have many payments
                   .WithOne(p => p.Booking)    // Each payment is associated with one booking
                   .HasForeignKey(p => p.BookingID) // Foreign key in Payment table
                   .OnDelete(DeleteBehavior.Cascade); // Enable cascade delete

            builder.Property(b => b.BookingDate)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(b => b.TotalPrice)
                   .HasPrecision(18, 6);

            // Add any other specific constraints as required
        }
    }
}
