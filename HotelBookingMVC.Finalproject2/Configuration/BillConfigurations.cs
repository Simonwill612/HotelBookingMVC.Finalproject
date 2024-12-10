using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using HotelBookingMVC.Finalproject2.Data.Entities;

namespace HotelBookingMVC.Finalproject2.Configuration
{
    public class BillConfiguration : IEntityTypeConfiguration<Bill>
    {
        public void Configure(EntityTypeBuilder<Bill> builder)
        {
            builder.HasKey(b => b.BillID);

            builder.Property(b => b.DateCreatedAt)
                   .IsRequired();

            builder.Property(b => b.DateUpdatedAt)
                   .IsRequired();

            builder.Property(b => b.FirstName)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(b => b.LastName)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.Property(b => b.Phone)
                  .IsRequired()
                  .HasMaxLength(100);

            builder.Property(b => b.Email)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(b => b.Address)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(b => b.Address2)
                   .HasMaxLength(200);

            builder.Property(b => b.Country)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(b => b.State)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(b => b.Zip)
                   .IsRequired()
                   .HasMaxLength(20);

            builder.Property(b => b.IsShippingSameAsBilling)
                   .IsRequired();

            builder.Property(b => b.SaveInfoForNextTime)
                   .IsRequired();

            builder.HasOne(b => b.Order)
                   .WithMany(o => o.Bills)
                   .HasForeignKey(b => b.OrderId)
                   .OnDelete(DeleteBehavior.Restrict); // Avoid cascading deletes
        }
    }
}
