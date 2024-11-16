using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelBookingMVC.Finalproject2.Data.Entities;

namespace HotelBookingMVC.Finalproject2.Configuration
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.DateCreated)
                   .IsRequired();

            builder.Property(o => o.Total)
                   .HasColumnType("decimal(18,2)");

            builder.Property(o => o.Tax)
                   .HasColumnType("decimal(18,2)");

            builder.Property(o => o.SubTotal)
                   .HasColumnType("decimal(18,2)");

            builder.Property(o => o.Phone)
                   .HasMaxLength(15);

            builder.Property(o => o.Address)
                   .HasMaxLength(500);

            builder.Property(o => o.Note)
                   .HasMaxLength(500);

            builder.HasMany(o => o.Bills)
                   .WithOne(b => b.Order)
                   .HasForeignKey(b => b.OrderId)
                   .OnDelete(DeleteBehavior.Restrict); // Avoid cascading deletes.
        }
    }
}
