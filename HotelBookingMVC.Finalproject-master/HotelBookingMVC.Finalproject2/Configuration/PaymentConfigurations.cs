using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelBookingMVC.Finalproject2.Data.Entities;

namespace HotelBookingMVC.Finalproject2.Configuration
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.PaymentID);

            builder.Property(p => p.PaymentDate)
                   .HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(p => p.Booking)
                   .WithMany(b => b.Payments)
                   .HasForeignKey(p => p.BookingID)
                   .OnDelete(DeleteBehavior.Restrict); // Avoid cascade delete.

            builder.Property(p => p.Amount)
                   .HasPrecision(18, 6);

            builder.Property(p => p.PaymentMethod)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(p => p.PaymentStatus)
                   .IsRequired()
                   .HasMaxLength(50);

            // Add further constraints if needed
        }
    }
}
